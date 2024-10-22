using Final;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;
namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        AuctionManager auctionManager;
        BidManager bidManager;
        ItemManager itemManager;
        public AuctionController(AuctionManager _auctionManager, BidManager _bidManager, ItemManager _itemManager)
        {
            this.auctionManager = _auctionManager;
            this.bidManager = _bidManager;
            this.itemManager = _itemManager;
        }

        [Authorize]
        [HttpGet("buyerAuctions")]
        public async Task<IActionResult> GetBuyerAuctions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBids = bidManager.GetAll().Where(b => b.BuyerID == userId);
            //auctions  that still active as its buyer id is null,and i shared on it by bids as the auction bid list contains atleast one bid of min
            var auctions = auctionManager.GetAll().Where(a => a.BuyerID == null && a.Bids.Any(b => userBids.Contains(b))).ToList();
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

        [Authorize]
        [HttpGet("lost")]
        public async Task<IActionResult> GetLost()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBids = bidManager.GetAll().Where(b => b.BuyerID == userId);
            //auctions  that i lost as its buyer id is not my id,and i shared on it by bids as the auction bid list contains atleast one bid of min
            var auctions = auctionManager.GetAll().Where(a => a.BuyerID != userId && a.BuyerID != null && a.Bids.Any(b => userBids.Contains(b))).ToList();
            return new JsonResult(auctions);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuction(AddAuctionModel _item)
        {


            var auction = await auctionManager.Add(_item.toAuctionModel());
            if (auction != null)
            {
                var item = await itemManager.GetOne(_item.ItemId);
                item.AuctionID = auction.ID;
                item.Auction = auction;
                await itemManager.Update(item);
            }
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
                var activeAuctions = allAuctions.List
                    .Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now && !a.Ended)
                    .ToList();

                // Paginate the filtered active auctions
                var paginatedActiveAuctions = activeAuctions
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // If no active auctions found
                if (!paginatedActiveAuctions.Any())
                {
                    return NotFound(new { Message = "No active auctions found." });
                }

                // Return the paginated active auction data
                var result = new
                {
                    List = paginatedActiveAuctions,
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

                // Apply filtering based on filterOption
                switch (filterOption.ToLower())
                {
                    case "open":
                        filteredAuctions = allAuctions.List
                            .Where(a => a.EndDate >= DateTime.Now && !a.Ended);
                        break;

                    case "closed":
                        filteredAuctions = allAuctions.List
                            .Where(a => a.EndDate < DateTime.Now || a.Ended);
                        break;

                    case "live":
                        filteredAuctions = allAuctions.List
                            .Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now && !a.Ended);
                        break;

                    default:
                        filteredAuctions = allAuctions.List; // No filter applied
                        break;
                }

                // Paginate the filtered auctions
                var paginatedAuctions = filteredAuctions
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

                var similarActiveAuctions = auctionManager.GetAll()
                    .Where(a => a.Item.CategoryID == auction.Item.CategoryID
                                && a.StartDate <= DateTime.Now
                                && a.EndDate >= DateTime.Now
                                && a.ID != auction.ID
                                && !a.Ended) // Exclude the auction itself
                    .OrderByDescending(a => a.ID)
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
            var auctions = auctionManager.GetAll();
            var ActiveAuctions = auctions.Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now && a.Ended == false).ToList();
            return Ok(ActiveAuctions);
        }

        [HttpGet("Ended")]
        public async Task<IActionResult> GetAllEnded()
        {
            var auctions = auctionManager.GetAll();
            var EndedAuctions = auctions.Where(a => a.EndDate < DateTime.Now).ToList();
            return Ok(EndedAuctions);
        }

        [HttpGet("SellerLive")]
        public async Task<IActionResult> SellerLiveAuction()
        {
            var SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var LiveAuctions = auctionManager.GetAll().Where(i => i.Item.SellerID == SellerId && i.StartDate <= DateTime.Now && i.EndDate >= DateTime.Now && i.Ended==false).ToList();
            return Ok(LiveAuctions);
        }


        [HttpGet("popularAuctions")]
        public async Task<IActionResult> PopularAuctions()
        {
            var popularAuctions = auctionManager.GetAll().OrderByDescending(a => a.Bids.Count).Where(a => a.Ended == false && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now && a.Bids.Count() > 0).ToList();
            return new JsonResult(popularAuctions);
        }



        [HttpGet("newArrivalsAuctions")]
        public async Task<IActionResult> NewArrivalsAuctions()
        {
            var newArrivals = auctionManager.GetAll().Where(a => !a.Ended && a.StartDate >= DateTime.Now.AddDays(-5) && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now).OrderByDescending(a => a.StartDate).ToList();
            return new JsonResult(newArrivals);
        }


        [HttpGet("endingSoon")]
        public async Task<IActionResult> EndingSoonAuctions()
        {
            var endingSoonAuctions = auctionManager.GetAll().Where(a => !a.Ended && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now && a.EndDate <= DateTime.Now.AddDays(2)).OrderByDescending(a => a.StartDate).ToList();
            return new JsonResult(endingSoonAuctions);
        }

        [HttpGet("noBids")]
        public async Task<IActionResult> NoBidsAuctions()
        {
            var noBidsAuctions = auctionManager.GetAll().Where(a => !a.Ended && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now && a.Bids.Count() == 0).ToList();
            return new JsonResult(noBidsAuctions);
        }

        [HttpGet("Close/{id}")]
        public async Task<IActionResult> CloseAuction(int id)
        {
            var auction = await auctionManager.GetOne(id);
            auction.Ended = true;
            var res = await auctionManager.Update(auction);
            return Ok(res);
        }


    }
}