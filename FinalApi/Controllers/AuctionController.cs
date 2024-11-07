using FinalApi;
using Hangfire;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ModelView;
using System.Security.Claims;
using FinalApi;
using Stripe;
using PayPal.Api;
using Microsoft.Extensions.Caching.Memory;
namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>
        {
            { "mode", "sandbox" },
        };

        AuctionManager auctionManager;
        BidManager bidManager;
        ItemManager itemManager;
        SellerManager sellerManager;
        ReviewManager reviewManager;
        private readonly PaymentManager paymentManager;
        private readonly HangfireManager hangfireManager;
        private readonly FavCategoryManager favCategoryManager;
        private readonly NotificationManager notificationManager;
        private readonly IMemoryCache memoryCache;
        private readonly IHubContext<NotificationsHub> hubContext;

        public AuctionController(AuctionManager _auctionManager, BidManager _bidManager,
            ItemManager _itemManager,PaymentManager _paymentManager,HangfireManager _hangfireManager,
            FavCategoryManager _favCategoryManager,NotificationManager _notificationManager,IMemoryCache _memoryCache,
            IHubContext<NotificationsHub> _hubContext,SellerManager _sellerManager , ReviewManager _reviewManager, IConfiguration _configuration)
        {
            this.auctionManager = _auctionManager;
            this.bidManager = _bidManager;
            this.itemManager = _itemManager;
            paymentManager = _paymentManager;
            hangfireManager = _hangfireManager;
            favCategoryManager = _favCategoryManager;
            notificationManager = _notificationManager;
            memoryCache = _memoryCache;
            hubContext = _hubContext;
            sellerManager = _sellerManager;
            this.reviewManager = _reviewManager;
            configuration = _configuration;

        }

        [Authorize]
        [HttpGet("buyerAuctions")]
        public async Task<IActionResult> GetBuyerAuctions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBids = bidManager.GetAll().Where(b => b.BuyerID == userId);
            //auctions  that still active as its buyer id is null,and i shared on it by bids as the auction bid list contains atleast one bid of min
            var auctions = auctionManager.GetAll().Where(a => a.BuyerID == null &&a.Ended==false&& a.Bids.Any(b => userBids.Contains(b))).Select(a=>a.ToCompletedAuctionVM()).ToList();
            return new JsonResult(auctions);
        }


        [Authorize]
        [HttpGet("won")]
        public async Task<IActionResult> GetWon()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auctions = auctionManager.GetAll().Where(a => a.BuyerID == userId).ToList();
            return new JsonResult(auctions);
        }

        [HttpGet("lost")]
        public async Task<IActionResult> GetLost()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBids = bidManager.GetAll().Where(b => b.BuyerID == userId);
            //auctions  that i lost as its buyer id is not my id,and i shared on it by bids as the auction bid list contains atleast one bid of min
            var auctions = auctionManager.GetAll().Where(a => a.BuyerID != userId && a.BuyerID != null&& a.Bids.Any(b => userBids.Contains(b))).Select(a=>a.SeeDetails()).ToList();
            return new JsonResult(auctions);
            /*
           var auction = auctionManager.GetAll().Where(a => a.BuyerID== userID && a.Payment.IsDone == false).ToList();
           if (auction == null)
               return BadRequest(new { message = "no lost auctions found" });

           List<LostAuctionViewModel> lostAuctions = new List<LostAuctionViewModel>();
           foreach (var item in auction)
               lostAuctions.Add(item.ToLostAuctionVM());

           if (auction != null)
               return new JsonResult(new ApiResultModel<List<LostAuctionViewModel>>
               {
                   result = lostAuctions,
                   success = true,
                   StatusCode = 200,
                   Message = "fetching data is completed"
               });
           return new JsonResult(new ApiResultModel<string>
           {
               result = "not lost auctions Here",
               success = false,
               StatusCode = 404,
               Message = "fetching data is not completed"
           });
       }
*/
        }

        [HttpPost]
        public async Task<IActionResult> AddAuction(AddAuctionModel _item)
        {
            var auction = await auctionManager.Add(_item.toAuctionModel());
            if (auction == null)
                return BadRequest(new { result = "auction not added" });

            var item = await itemManager.GetOne(_item.ItemId);
            item.AuctionID = auction.ID;
            item.Auction = auction;
            await itemManager.Update(item);

            BackgroundJob.Schedule(() => hangfireManager.EndAuctionAtTime(auction.ID), auction.EndDate);

            BackgroundJob.Enqueue(() => hangfireManager.SendNotificationsToUserInFavCategory(item.CategoryID, auction.ID));

            BackgroundJob.Schedule(() => hangfireManager.AuctionEndedNotificationBeforeOneDay(auction.ID),
                            DateTime.Now.AddDays(auction.EndDate.Day - 1));
            //check favCategory if have user send them that auction add in these category
            return new JsonResult(new ApiResultModel<string> { result = "auction added successfully" });
        }

        [HttpGet("GetAuctions")]
        public IActionResult GetActiveAuctions(string searchtxt = "", string columnName = "Id", bool isAscending = false, int pageSize = 2, int pageNumber = 1, string categoryName = "", string filterOption = "")
        {
            try
            {
                if (pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page size must be greater than zero." });
                }
                if (pageNumber <= 0)
                {
                    return BadRequest(new { Message = "Page number must be greater than zero." });
                }

                var allAuctions = auctionManager.Get(
                    searchtxt,
                    columnName,
                    isAscending,
                    int.MaxValue,
                    1,
                    categoryName,
                    filterOption
                    );

                // Filter for active auctions
                var currentUtcTime = DateTime.UtcNow;

                var activeAuctions = allAuctions.List
                    .Where(a => a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime && !a.Ended)
                    .ToList();

                // Paginate the filtered active auctions
                var paginatedActiveAuctions = activeAuctions
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // If no active auctions found
                if (!paginatedActiveAuctions.Any())
                {
                    // Return the paginated active auction data

                    return Ok(new
                    {
                        List = paginatedActiveAuctions.Select(a => a.SeeDetails()),
                        TotalCount = activeAuctions.Count
                    });
                }

                // Return the paginated active auction data
                var result = new
                {
                    List = paginatedActiveAuctions.Select(a=>a.SeeDetails()),
                    TotalCount = activeAuctions.Count 
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching active auctions.", Error = ex.Message });
            }
        }


        [HttpGet("GetAuctionForAdmin")]
        public IActionResult GetAuctionForAdmin(string searchtxt = "", string columnName = "Id", bool isAscending = false, int pageSize = 2, int pageNumber = 1, string categoryName = "", string filterOption = "")
        {
            try
            {
                if (pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page size must be greater than zero." });
                }
                if (pageNumber <= 0)
                {
                    return BadRequest(new { Message = "Page number must be greater than zero." });
                }

                var allAuctions = auctionManager.Get(
                    searchtxt,
                    columnName,
                    isAscending,
                    int.MaxValue,
                    1,
                    categoryName,
                    filterOption
                    );

                IEnumerable<Auction> filteredAuctions;
                var currentUtcTime = DateTime.UtcNow;
                // Apply filtering based on filterOption
                switch (filterOption.ToLower())
                {
                    case "open":
                        filteredAuctions = allAuctions.List
                            .Where(a => a.EndDate >= currentUtcTime && !a.Ended);
                        break;

                    case "closed":
                        filteredAuctions = allAuctions.List
                            .Where(a => a.EndDate < currentUtcTime || a.Ended);
                        break;

                    case "live":
                        filteredAuctions = allAuctions.List
                           .Where(a => a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime && !a.Ended);
                        break;
                    case "paid":
                        filteredAuctions = allAuctions.List
                            .Where(a => a.Completed); 
                        break;

                    case "unpaid":
                        filteredAuctions = allAuctions.List
                            .Where(a => !a.Completed);
                        break;

                    default:
                        filteredAuctions = allAuctions.List; // No filter applied
                        break;
                }

                // Paginate the filtered auctions
                var paginatedAuctions = filteredAuctions.Select(a=>a.SeeDetails())
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // If no auctions found after filtering and pagination
                if (!paginatedAuctions.Any())
                {
                    return NotFound(new { Message = "No auctions found based on the given filter." });
                }

                // Return the paginated auction data
                var result = new
                {
                    List = paginatedAuctions,
                    TotalCount = filteredAuctions.Count()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching auctions.", Error = ex.Message });
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetAuctionById(int id)
        {
            try
            {
                var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == id).SeeDetails();

                decimal currentPrice = auction.Item.StartPrice;
                
                foreach(var bid in auction.Bids)
                {
                    currentPrice += bid.Amount;
                    bid.Amount = currentPrice;
                }
                
                if (auction == null)
                {
                    return NotFound(new { Message = $"Auction with ID {id} not found." });
                }

                return Ok(auction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the auction.", Error = ex.Message });
            }
        }

        [HttpGet("SimilarActiveAuctions/{id}")]
        public async Task<IActionResult> GetSimilarActiveAuctions(int id)
        {
            try
            {
                var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == id);

                if (auction == null)
                {
                    return NotFound(new { Message = $"Auction with ID {id} not found." });
                }
                var currentUtcTime = DateTime.UtcNow;
                var similarActiveAuctions = auctionManager.GetAll()
                    .Where(a => a.Item.CategoryID == auction.Item.CategoryID
                                && a.StartDate <= currentUtcTime
                                && a.EndDate >= currentUtcTime
                                && a.ID != auction.ID
                                && !a.Ended) // Exclude the auction itself
                    .OrderByDescending(a => a.ID).Select(a=>a.SeeDetails())
                    .Take(3)
                    .ToList();

                if (similarActiveAuctions == null || !similarActiveAuctions.Any())
                {
                    return NotFound(new { Message = "No similar active auctions found." });
                }

                return Ok(similarActiveAuctions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching similar auctions.", Error = ex.Message });
            }
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetAllActive()
        {
            var currentUtcTime = DateTime.UtcNow;

            var auctions = auctionManager.GetAll();
            var ActiveAuctions = auctions.Where(a => a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime && a.Ended==false).Select(a => a.SeeDetails()).ToList();
            return Ok(ActiveAuctions);
        }
        [HttpGet("HomeActive")]
        public async Task<IActionResult> HomeGetAllActive()
        {
            var currentUtcTime = DateTime.UtcNow;

            var auctions = auctionManager.GetAll();
            var ActiveAuctions = auctions.Where(a => a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime && a.Ended == false).Select(a => a.SeeDetails())
                .Take(6)
                .ToList();
            return Ok(ActiveAuctions);
        }

        [HttpGet("Ended")]
        public async Task<IActionResult> GetAllEnded()
        {
            var currentUtcTime = DateTime.UtcNow;
            var auctions = auctionManager.GetAll();
            var EndedAuctions = auctions.Where(a => a.EndDate < currentUtcTime).Select(a => a.SeeDetails()).ToList();
            return Ok(EndedAuctions);
        }
        
        [Authorize]
        [HttpGet("SellerLive")]
        public async Task<IActionResult> SellerLiveAuction()
        {
            var currentUtcTime = DateTime.UtcNow;

            var SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var LiveAuctions = auctionManager.GetAll().Where(i => i.Item.SellerID == SellerId && i.StartDate <= currentUtcTime && i.EndDate >= currentUtcTime && i.Ended==false).Select(a => a.SeeDetails()).ToList();
            return Ok(LiveAuctions);
        }
        [Authorize]
        [HttpGet("SellerUpcoming")]
        public async Task<IActionResult> SellerUpComingAuctions()
        {
            var currentUtcTime = DateTime.UtcNow;

            var SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var upcomingAuctions = auctionManager.GetAll().Where(i => i.Item.SellerID == SellerId && i.StartDate > currentUtcTime && i.EndDate >= currentUtcTime && i.Ended == false).Select(a=>a.SeeDetails()).ToList();
            return Ok(upcomingAuctions);
        }
        [HttpGet("UpcomingAuctions")]
        public async Task<IActionResult> UpComingAuctions()
        {
            var currentUtcTime = DateTime.UtcNow;
            var upcomingAuctions = auctionManager.GetAll()
                .Where(i => i.StartDate > currentUtcTime && i.EndDate >= currentUtcTime && !i.Ended)
                .Select(a => a.SeeDetails())
                 .Take(6)
                .ToList();
            return Ok(upcomingAuctions);
        }

        [HttpGet("popularAuctions")]
        public async Task<IActionResult> PopularAuctions()
        {
            var currentUtcTime = DateTime.UtcNow;
            var popularAuctions = auctionManager.GetAll()
                .Where(a => !a.Ended && a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime && a.Bids.Count() > 0)
                .OrderByDescending(a => a.Bids.Count)
                .Select(a => a.SeeDetails())
                .Take(6)
                .ToList();
            return new JsonResult(popularAuctions);
        }

        [HttpGet("newArrivalsAuctions")]
        public async Task<IActionResult> NewArrivalsAuctions()
        {
            var currentUtcTime = DateTime.UtcNow;
            var newArrivals = auctionManager.GetAll()
                .Where(a => !a.Ended && a.StartDate >= currentUtcTime.AddDays(-5) && a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime)
                .OrderByDescending(a => a.StartDate)
                .Select(a => a.SeeDetails())
                .Take(6)
                .ToList();
            return new JsonResult(newArrivals);
        }

        [HttpGet("endingSoon")]
        public async Task<IActionResult> EndingSoonAuctions()
        {
            if(memoryCache.TryGetValue("endingSoon",out var resultCache))
                return new JsonResult(resultCache);
            var currentUtcTime = DateTime.UtcNow;
            var endingSoonAuctions = auctionManager.GetAll()
                .Where(a => !a.Ended && a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime && a.EndDate <= currentUtcTime.AddDays(1))
                .OrderByDescending(a => a.StartDate)
                .Select(a => a.SeeDetails())
                .Take(6)
                .ToList();
            //memoryCache.Set("endingSoon", endingSoonAuctions, new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            //});
            return new JsonResult(endingSoonAuctions);
        }

        [HttpGet("noBids")]
        public async Task<IActionResult> NoBidsAuctions()
        {
            var currentUtcTime = DateTime.UtcNow;
            var noBidsAuctions = auctionManager.GetAll()
                .Where(a => !a.Ended && a.StartDate <= currentUtcTime && a.EndDate >= currentUtcTime && a.Bids.Count() == 0)
                .Select(a => a.SeeDetails())
                .Take(6)
                .ToList();

            return new JsonResult(noBidsAuctions);
        }



        [HttpGet("AllDoneAuctions")]
        public IActionResult AllDoneAuctions()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userID))
                return BadRequest(new { message = "the user not found" });

            var auctions = auctionManager.GetAll()
                .Where(a => a.BuyerID == userID && a.Payment.IsDone == true)
                .ToList();

            if (!auctions.Any())
                return BadRequest(new { message = "no done auctions found" });

            List<DoneAuctionViewModel> doneAuctions = new List<DoneAuctionViewModel>();

            foreach (var auction in auctions)
            {
                // Check if the user has added a review for this auction
                bool isReviewed = reviewManager.HasUserReviewedAuction(userID, auction.ID);

                // Map auction to DoneAuctionViewModel and set IsReviewed property
                var doneAuctionVM = auction.ToDoneAuctionVM();
                doneAuctionVM.IsReviewd = isReviewed;
                doneAuctionVM.AuctionID = auction.ID;

                doneAuctions.Add(doneAuctionVM);
            }

            return new JsonResult(new ApiResultModel<List<DoneAuctionViewModel>>
            {
                result = doneAuctions,
                success = true,
                StatusCode = 200,
                Message = "fetching data is completed"
            });
        }


        [HttpGet("AllCompletedAuctions")]
        public IActionResult AllCompletedAuctions()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userID))
                return BadRequest(new { message = "the user not found" });
            var auctions = auctionManager.GetAll().Where(a => a.Item.SellerID == userID && a.Completed == true).Select(a=>a.ToCompletedAuctionVM()).ToList();

           

            if (auctions.Any())
                return new JsonResult(new ApiResultModel<List<CompletedAuctionViewModel>>
                {
                    result = auctions,
                    success = true,
                    StatusCode = 200,
                    Message = "fetching data is completed"
                });
            return new JsonResult(new ApiResultModel<string>
            {
                result = null,
                success = false,
                StatusCode = 404,
                Message = "fetching data is not completed"
            });
        }
        [HttpGet("AllInCompletedAuctions")]
        public IActionResult AllInCompletedAuctions()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userID))
                return BadRequest(new { message = "the user not found" });
            var auctions = auctionManager.GetAll().Where(a => a.Item.SellerID == userID && a.Completed == false && a.Ended==true).Select(a => a.ToCompletedAuctionVM()).ToList();



            if (auctions.Any())
                return new JsonResult(new ApiResultModel<List<CompletedAuctionViewModel>>
                {
                    result = auctions,
                    success = true,
                    StatusCode = 200,
                    Message = "fetching data is completed"
                });
            return new JsonResult(new ApiResultModel<string>
            {
                result = null,
                success = false,
                StatusCode = 404,
                Message = "fetching data is not completed"
            });
        }


        [HttpGet("getAvailableBalance")]
        public async Task<IActionResult> GetAvailableBalance()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userID))
                return BadRequest(new { message = "the user not found" });
            var auctions = auctionManager.GetAll().Where(a => a.Item.SellerID == userID && a.Completed == true).Select(a => a.ToCompletedAuctionVM()).ToList();
            decimal availableBalance = auctions.Sum(a => a.totalPrice);
            var seller =await sellerManager.GetOne(userID);
            var withdrawnAmount = (decimal)seller.WithdrawnAmount;
            if (withdrawnAmount != null)
            { availableBalance = auctions.Sum(a => a.totalPrice) -withdrawnAmount; }
           
                return new JsonResult(new ApiResultModel<object>
                {
                    result = new
                    {availablebalance = availableBalance,PaymentEmail = new { paypalEmail = seller.User.PaypalEmail, StripeEmail = seller.User.StripeEmail } },
                        success = true,
                        StatusCode = 200,
                        Message = "fetching data is completed"
                    });   
        }

        [HttpGet("withdraw/{amount}")]
        public async Task<IActionResult> WithDraw(decimal amount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auctions = auctionManager.GetAll().Where(a => a.Item.SellerID == userId && a.Completed == true).Select(a => a.ToCompletedAuctionVM()).ToList();
           
            // get the available balance to compare it with the amount will be witdrawn only
            var availableBalance = auctions.Sum(a => a.totalPrice) - await GetWithdrawnAmount(userId);
            if (amount > availableBalance)
            {
                return BadRequest(new {message= "insufficient balance" });
            }
            UpdateWithdrawnAmount(userId, amount);
            try
            {
                var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPalSetting:ClientID"],
                    configuration["PayPalSetting:Secret"], Config).GetAccessToken());

                var payment = new PayPal.Api.Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        total = amount.ToString("F2"),
                        currency = "USD",
                    },
                    description = "Auction deposit"
                }
            },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = $"http://localhost:4200/user/seller-earnings",
                        cancel_url = "http://localhost:4200/user/seller-withdraw"
                    }
                };

                var createdPayment = payment.Create(apiContext).GetApprovalUrl();

                await hubContext.Clients.All.SendAsync("withdrawn amount", amount);
                return Ok(new { result = createdPayment, status = 200 });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "withdrawing is not completed" });
            }

            //return Ok(new {message="withdrawn successfully"});
        }

        private async Task<decimal> GetWithdrawnAmount(string userID)
        {
            var seller =await sellerManager.GetOne(userID);
            var withdrawnAmount = seller.WithdrawnAmount;


            return (decimal)withdrawnAmount; 
            
            
        }

        [HttpGet("getwithdrawnamount")]
        public async Task<IActionResult> GetWithdrawnAmount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var seller = await sellerManager.GetOne(userId);
            var withdrawnAmount = seller.WithdrawnAmount;

            return Ok(withdrawnAmount);
        }
        private async Task<IActionResult> UpdateWithdrawnAmount(string userID,decimal _amount)
        {
            var seller = await sellerManager.GetOne(userID);
            seller.WithdrawnAmount += _amount;
           var result= sellerManager.Update(seller);

            return Ok(result);
        }

        [HttpGet("getUpComingBalance")]
        public IActionResult GetUpcomingBalance()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userID))
                return BadRequest(new { message = "the user not found" });
            var auctions = auctionManager.GetAll().Where(a => a.Item.SellerID == userID && a.Completed == false && a.Ended == true).Select(a => a.ToCompletedAuctionVM()).ToList();
            decimal upcomingBalance = 0;
            foreach (var auction in auctions)
            {
                upcomingBalance += auction.totalPrice;
            }

            if (auctions.Any())
                return new JsonResult(new ApiResultModel<decimal>
                {
                    result = upcomingBalance,
                    success = true,
                    StatusCode = 200,
                    Message = "fetching data is completed"
                });
            return new JsonResult(new ApiResultModel<string>
            {
                result = null,
                success = false,
                StatusCode = 404,
                Message = "fetching data is not completed"
            });
        }

        [HttpGet("CompleteAuctionPayment/{_itemID:int}")]
        public IActionResult CompleteAuctionPayment(int _itemID)
        {
            var item = itemManager.GetAll().FirstOrDefault(i => i.ID == _itemID);
            if (item == null) return BadRequest(new { message = "no item found to this user" });

            string buyerID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerID)) return BadRequest(new { message = "user not found" });

            decimal bidsAmount = 0;
            var bids = bidManager.GetAll().Where(b => b.AuctionID==item.AuctionID);

            if (bids.Any()) foreach (var bid in bids) bidsAmount += bid.Amount;
            
            return new JsonResult(new ApiResultModel<CompleteAuctionPaymentViewModel>
            {
                result = item.ToCompleteAuctionPayment(bidsAmount),
                success = true,
                StatusCode = 200,
                Message = "fetching data is completed"
            });
        }

        [HttpGet("Close/{id}")]
        public async Task<IActionResult> CloseAuction(int id)
        {
            var auction = await auctionManager.GetOne(id);
            auction.Ended = true;
            var res = await auctionManager.Update(auction);

            var refundAmount = auction.Item.StartPrice;
            var lostBuyersEmails = paymentManager.GetAll().Where(p => p.AuctionID == id && p.IsDone == false).Count();
            var result = paymentManager.RefundCustomerAmount("gamal-gamal@personal.example.com", refundAmount * lostBuyersEmails);
            var uniqueBuyerIds = auction.Bids.Select(i => i.BuyerID).Distinct().ToList();
            List<Buyer> buyers = new List<Buyer>();

            foreach (var uniqueBuyerId in uniqueBuyerIds)
            {
                var buyer = auction.Bids.Where(i => i.BuyerID == uniqueBuyerId).Select(i => i.Buyer).FirstOrDefault();
                if (buyer != null)
                {
                    buyers.Add(buyer);
                }
            }


            foreach (var Buyer in buyers)
            {
                if (await notificationManager.Add(new Notification
                {
                    Title = Enums.NotificationType.auction,
                    UserId = Buyer.UserID,
                    Date = DateTime.Now,
                    Description = $"the seller of {auction.Item.Name} auction closed that auction",
                    IsReaded = false,
                })
                   )
                {
                    var lastNotification = notificationManager.GetAll()
                        .Where(n => n.UserId == Buyer.UserID)
                        .OrderBy(n => n.Id).LastOrDefault();
                    await hubContext.Clients.Group(Buyer.UserID).SendAsync("notification", lastNotification.ToViewModel());
                }

            }

            return Ok(res);
        }


    }
}