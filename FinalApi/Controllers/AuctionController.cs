using Managers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using ModelView;
namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        AuctionManager auctionManager;
        public AuctionController(AuctionManager _auctionManager)
        {
            this.auctionManager = _auctionManager;
        }


        //[HttpGet("getall")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var auctions =  auctionManager.GetAll();
        //    return Ok(auctions);
        //}

        [HttpGet("GetAuctions")]
        public IActionResult GetAuctions(string searchtxt = "", string columnName = "Id", bool isAscending = false, int pageSize = 2, int pageNumber = 1)
        {
            try
            {
                // Call the service/repository method to fetch paginated auction data
                var paginatedAuctions = auctionManager.Get(searchtxt, columnName, isAscending, pageSize, pageNumber);

                // If no auctions found
                if (paginatedAuctions == null || !paginatedAuctions.List.Any())
                {
                    return NotFound(new { Message = "No auctions found." });
                }

                // Return the paginated auction data
                return Ok(paginatedAuctions);
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, new { Message = "An error occurred while fetching auctions.", Error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddAuction(AddAuctionModel _item)
        {

            var result = await auctionManager.Add(_item.toAuctionModel());
            if (result == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetAllActive()
        {
            var auctions = auctionManager.GetAll();
            var ActiveAuctions = auctions.Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now).ToList();
            return Ok(ActiveAuctions);
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
                // Fetch the auction by ID using the auction manager
                var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == id);


                // Check if the auction was found
                if (auction == null)
                {
                    return NotFound(new { Message = $"Auction with ID {id} not found." });
                }

                // Return the auction details
                return Ok(auction);
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, new { Message = "An error occurred while fetching the auction.", Error = ex.Message });
            }
        }

    }
}