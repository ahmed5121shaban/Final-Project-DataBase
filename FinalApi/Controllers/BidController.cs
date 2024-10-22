﻿using Final;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ModelView;
using System.Security.Claims;
using System.Text;

namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        
        private readonly UserManager<User> userManager;
        private readonly PaymentManager paymentManager;
        private readonly BidManager bidManager;
        private readonly ItemManager itemManager;
        private readonly BidsHub bidsHub;
        private readonly IHubContext<BidsHub> hubContext;
        private readonly FavAuctionManager favAuctionManager;
        private readonly NotificationManager notificationManager;

        public BidController(UserManager<User> _userManager,PaymentManager _paymentManager,
            BidManager _bidManager,ItemManager _itemManager, IHubContext<BidsHub> _hubContext,
            FavAuctionManager _favAuctionManager,NotificationManager _notificationManager)
        {
            userManager = _userManager;
            paymentManager = _paymentManager;
            bidManager = _bidManager;
            itemManager = _itemManager;
            hubContext = _hubContext;
            favAuctionManager = _favAuctionManager;
            notificationManager = _notificationManager;
        }


        [HttpPost("auction-payment-method")]
        public IActionResult AuctionPaymentMethod([FromBody]AddPaymentViewModel _addPaymentView)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userManager.Users.FirstOrDefault( u => u.Id == Id );
            if (user == null) 
            {
                return NotFound();
            }
            
            paymentManager.Add(_addPaymentView);

            return Ok();
        }

        [HttpPost("first-auction-payment")]
        public async Task<IActionResult> MinceFirstAuctionPayment([FromBody]PaymentStartPriceViewModel _paymentView)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "this data is not completed" , ModelState });

            var userID =User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
                return BadRequest(new { message = "the user not found" });

            _paymentView.BuyerID = userID;

            var user = userManager.Users.FirstOrDefault(u=>u.Id==userID);
                
           
            _paymentView.Currency = user?.Currency ?? "USD";
            string result= string.Empty;
            if (_paymentView.Method == Enums.PaymentMetod.paypal)
            {
                result = await bidManager.MinceAuctionStartPrice(_paymentView);
                if (!string.IsNullOrEmpty(result)) return Ok(new { status = 200, result });
                return BadRequest(new { message = "the PayPal payment is not completed" });
            }
            result = await bidManager.MinceAuctionStartPrice(_paymentView);
            if (!string.IsNullOrEmpty(result)) return Ok(new { status = 200, result });
            return BadRequest(new { message = "the Stripe payment is not completed" });

        }


        [HttpGet("user-have-payment/{itemID:int}")]
        public IActionResult GetPaymentMethodsCount(int itemID) 
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
                return new JsonResult(new {message="the user not found", count = 4 });
            var item = itemManager.GetAll().FirstOrDefault(i=>i.ID == itemID&&i.SellerID== userID);
            if (User.IsInRole("Seller")&&item!=null)
                return new JsonResult( new { message = "not allowed" ,count=0} );

            var payments = paymentManager.GetAll().Where(p=>p.BuyerId == userID).ToList();

            if (payments.Count==0)
               return new JsonResult(new { message = "the user don't have any Payment Method", count = 1 });
            else if (payments.Count > 1)
                return new JsonResult(new { message = "the user have more than one Payment Method", count = 2 });
            else 
                return new JsonResult(new { message = "the user have one Payment Method", count = 3, method = payments.Select(p => p.Method) });

        }


        [HttpGet("all-bids-auction{auctionID:int}")]
        public IActionResult GetBids(int auctionID)
        {
            var bids = bidManager.GetAll().Where(b => b.AuctionID == auctionID).ToList();
            if (bids == null)
                return BadRequest(new {message="no bids in this auction"});
            return Ok(bids);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(AddBidViewModel _addBidView)
        {
          
            if (ModelState.IsValid)
            {
                _addBidView.BuyerID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _addBidView.Time = DateTime.Now;

                var res =await bidManager.Add(_addBidView.ToModel());
                if (res)
                {
                    /*List<string> usersIDs = favAuctionManager.GetAll().Where(f=>f.AuctionID== _addBidView.AuctionID)
                        .Select(f=>f.BuyerID).ToList();
                    if (usersIDs.Any())
                    {
                        foreach (var id in usersIDs)
                        {
                            if (await notificationManager.Add(new Notification
                            {
                                Title = Enums.NotificationType.auction,
                                UserId = id,
                                Date = DateTime.Now,
                                Description = "New Auction Added",
                                IsReaded = false,
                            }))
                                await hubContext.Clients.Groups(id).SendAsync("receive", new
                                {
                                    Title = Enums.NotificationType.auction,
                                    Date = DateTime.Now,
                                    Description = "New Bid Added",
                                    IsReaded = false,
                                });
                        }
                    }*/

                    //when i call this func it be error
                    var bids = bidManager.GetAll().Where(b => b.AuctionID == _addBidView.AuctionID).ToList();

                    List<BidViewModel> bidViewModels = new List<BidViewModel>();
                    foreach (var bid in bids)
                        bidViewModels.Add(bid.ToBidViewModel());
                    
                    await hubContext.Clients.Group(_addBidView.AuctionID.ToString()).SendAsync("AllBids", bidViewModels);

                    return new JsonResult(new ApiResultModel<bool>()
                    {
                        result = res,
                        StatusCode = 200,
                        success = true,
                        Message = "done successfully"

                    });
                    
                }
                else
                {
                    return new JsonResult(new ApiResultModel<bool>()
                    {
                        result = res,
                        StatusCode = 400,
                        success = false,
                        Message = "An Error Has Occured"

                    });
                }
            }
            else
            {
                var builder = new StringBuilder();
                foreach (var item in ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        builder.Append(builder.ToString());
                    }
                }
                return new JsonResult(new ApiResultModel<string>()
                {
                    result = builder.ToString(),
                    StatusCode = 400,
                    success = false,
                    Message = "Not Valid Model"
                }); 
            }
        }

        [HttpGet]
        [Route("get-highest-bid/{AuctionId}")]
        public IActionResult GetHighestBid(int AuctionId)
        {
            var res = bidManager.GetHighest(AuctionId);
            if (res != null)
            {
                return new JsonResult(new ApiResultModel<Bid>()
                {
                    result = res,
                    StatusCode = 200,
                    success = true,
                    Message = "done successfully"

                });
            }
            else
            {
                return new JsonResult(new ApiResultModel<string>()
                {
                    result = "",
                    StatusCode = 400,
                    success = false,
                    Message = "An Error Has Occured"

                });

            }
        }
    }
}
