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
            var auctions = auctionManager.GetAll().Where(a => a.BuyerID != userId && a.Bids.Any(b => userBids.Contains(b))).ToList();
            return new JsonResult(auctions);
        }


        //[HttpGet("GetAuctions")]
        //public IActionResult GetAuctions(string searchtxt = "", string columnName = "Id", bool isAscending = false, int pageSize = 2, int pageNumber = 1, string categoryName = "")
        //{
        //    try
        //    {
        //        var paginatedAuctions = auctionManager.Get(searchtxt, columnName, isAscending, pageSize, pageNumber, categoryName);

        //        // If no auctions found
        //        if (paginatedAuctions == null || !paginatedAuctions.List.Any())
        //        {
        //            return NotFound(new { Message = "No auctions found." });
        //        }

        //        // Return the paginated auction data
        //        return Ok(paginatedAuctions);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any errors
        //        return StatusCode(500, new { Message = "An error occurred while fetching auctions.", Error = ex.Message });
        //    }
        //}

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
        public IActionResult GetActiveAuctions(string searchtxt = "", string columnName = "Id", bool isAscending = false, int pageSize = 2, int pageNumber = 1, string categoryName = "", string filterOption ="")
        {
            try
            {
                // Validate page size and page number
                if (pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page size must be greater than zero." });
                }
                if (pageNumber <= 0)
                {
                    return BadRequest(new { Message = "Page number must be greater than zero." });
                }

                // Get active auctions directly
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
                    .Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now)
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
                    TotalCount = activeAuctions.Count // Return the total count of active auctions for pagination
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, new { Message = "An error occurred while fetching active auctions.", Error = ex.Message });
            }
        }


        [HttpGet("GetEndedAuctions")]
        public IActionResult GetEndedAuctions(string searchtxt = "", string columnName = "Id", bool isAscending = false, int pageSize = 2, int pageNumber = 1, string categoryName = "")
        {
            try
            {
                var allAuctions = auctionManager.Get(searchtxt, columnName, isAscending, int.MaxValue, 1, categoryName);

                var EndedAuctions = allAuctions.List.Where(a => a.EndDate < DateTime.Now).ToList();

                var paginatedEndedAuctions = EndedAuctions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                if (!paginatedEndedAuctions.Any())
                {
                    return NotFound(new { Message = "No active auctions found." });
                }

                var result = new
                {
                    List = paginatedEndedAuctions,
                    TotalCount = EndedAuctions.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, new { Message = "An error occurred while fetching active auctions.", Error = ex.Message });
            }
        }


        [HttpGet("Ended")]
        public async Task<IActionResult> GetAllEnded()
        {
            var auctions = auctionManager.GetAll();
            var EndedAuctions = auctions.Where(a => a.EndDate < DateTime.Now).ToList();
            return Ok(EndedAuctions);
        }

        // get auction by ID
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetAuctionById(int id)
        {
            try
            {
                var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == id);

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
                                && a.ID != auction.ID) // Exclude the auction itself
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
            var ActiveAuctions = auctions.Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now).ToList();
            return Ok(ActiveAuctions);
        }

        //[HttpGet("Ended")]
        //public async Task<IActionResult> GetAllEnded()
        //{
        //    var auctions = auctionManager.GetAll();
        //    var EndedAuctions = auctions.Where(a => a.EndDate < DateTime.Now).ToList();
        //    return Ok(EndedAuctions);
        //}

        // get auction by ID
        //[HttpGet("GetById/{id}")]
        //public async Task<IActionResult> GetAuctionById(int id)
        //{
        //    try
        //    {
        //        // Fetch the auction by ID using the auction manager
        //        var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == id);


        //        // Check if the auction was found
        //        if (auction == null)
        //        {
        //            return NotFound(new { Message = $"Auction with ID {id} not found." });
        //        }

        //        // Return the auction details
        //        return Ok(auction);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any errors
        //        return StatusCode(500, new { Message = "An error occurred while fetching the auction.", Error = ex.Message });
        //    }
        //}

        [HttpGet("SellerLive")]
        public async Task<IActionResult> SellerLiveAuction()
        {
            var SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var LiveAuctions = auctionManager.GetAll().Where(i => i.Item.SellerID == SellerId && i.StartDate <= DateTime.Now && i.EndDate >= DateTime.Now).ToList();
            return Ok(LiveAuctions);
        }
    }
}