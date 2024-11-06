using FinalApi;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache memoryCache;
        private readonly FavAuctionManager favAuctionManager;
        private readonly NotificationManager notificationManager;
        private readonly IHubContext<DashboardHub> dashboardHubContext;

        public BidController(UserManager<User> _userManager,PaymentManager _paymentManager,
            BidManager _bidManager,ItemManager _itemManager, IHubContext<BidsHub> _hubContext,IMemoryCache _memoryCache,
            FavAuctionManager _favAuctionManager,NotificationManager _notificationManager,IHubContext<DashboardHub> _dashboardHubContext)
        {
            userManager = _userManager;
            paymentManager = _paymentManager;
            bidManager = _bidManager;
            itemManager = _itemManager;
            hubContext = _hubContext;
            memoryCache = _memoryCache;
            favAuctionManager = _favAuctionManager;
            notificationManager = _notificationManager;
            dashboardHubContext = _dashboardHubContext;
        }



        [HttpGet("bids/{auctionID:int}")]
        public IActionResult GetBids(int auctionID)
        {
            
            List<BidViewModel> bidViewModels = new List<BidViewModel>();
            foreach (var bid in bidManager.GetAll().Where(b => b.AuctionID == auctionID).ToList())
            {
                bidViewModels.Add(bid.ToBidViewModel());
            }
            if (!bidViewModels.Any())
                return BadRequest(new { message = "no bids" });
            return new JsonResult(new ApiResultModel<List<BidViewModel>>
            {
                Message = "getting bids completed",
                result = bidViewModels,
                success = true,
                StatusCode = 200

            });
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


        [HttpGet("user-have-payment/{itemID:int}/{auctionID:int}")]
        public async Task<IActionResult> GetPaymentMethodsCount(int itemID,int auctionID) 
        {

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
                return new JsonResult(new {message="the user not found", count = 4 });
            var item = itemManager.GetAll().FirstOrDefault(i=>i.ID == itemID&&i.SellerID== userID);
            if (item!=null)
                return new JsonResult( new { message = "not allowed" ,count=0} );

            var user =await userManager.FindByIdAsync(userID);
            var payment = paymentManager.GetAll().Where(p => p.BuyerId == userID && p.AuctionID == auctionID).FirstOrDefault();
            if (payment != null)
                return new JsonResult(new { message = "the user don't have any Payment Method", count = 5 ,method=payment.Method});

            if (user.PaypalEmail == null && user.StripeEmail == null)
                return new JsonResult(new { message = "the user don't have any Payment Method", count = 1 });
            else if (user.PaypalEmail != null && user.StripeEmail != null)
                return new JsonResult(new { message = "the user have more than one Payment Method", count = 2 });
            else if(user.PaypalEmail!=null)
                return new JsonResult(new { message = "the user have one Payment Method", count = 3, method = Enums.PaymentMetod.paypal });
            else
                return new JsonResult(new { message = "the user have one Payment Method", count = 3, method = Enums.PaymentMetod.stripe });
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
                    var bids = bidManager.GetAll().Where(b => b.AuctionID == _addBidView.AuctionID);
                    List<BidViewModel> bidViewModels = new List<BidViewModel>();
                    foreach (var bid in bids)
                        bidViewModels.Add(bid.ToBidViewModel());
                        
                    
                    await hubContext.Clients.Group(_addBidView.AuctionID.ToString()).SendAsync("AllBids", bidViewModels);
                    var itemName = bids.Select(b => b.Auction.Item.Name).FirstOrDefault();
                    var allAmount = bids.Select(b => b.Amount).Sum() + bids.Select(b => b.Auction.Item.StartPrice).FirstOrDefault();
                    var auctionBidAmount = new { name = itemName, value= allAmount };
                    await dashboardHubContext.Clients.All.SendAsync("auctionsBidsAmount", auctionBidAmount);
                    
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
