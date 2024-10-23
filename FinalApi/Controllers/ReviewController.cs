using Final;
using Managers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private ReviewManager reviewManager;
        private AuctionManager auctionManager;
        private ItemManager itemManager;
        public ReviewController(
            ReviewManager _reviewManager,
           AuctionManager _auctionManager,
           ItemManager _itemManager) {
        this.reviewManager = _reviewManager;
            this.auctionManager = _auctionManager;
            this.itemManager = _itemManager;
        
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetReviews()
        {

            var reviews = reviewManager.GetAll().ToList();
            return new JsonResult(reviews);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var buyerID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (buyerID != null)
                {
                    var auction = await auctionManager.GetOne(model.AuctionID);
                    if (auction == null)
                    {
                        return new JsonResult(new ApiResultModel<bool>
                        {
                            result = false,
                            StatusCode = StatusCodes.Status404NotFound,
                            success = false,
                            Message = "Auction not found."
                        });
                    }

                    var item = itemManager.GetAll().FirstOrDefault(i => i.ID == auction.ItemID);
                    if (item == null)
                    {
                        return new JsonResult(new ApiResultModel<bool>
                        {
                            result = false,
                            StatusCode = StatusCodes.Status404NotFound,
                            success = false,
                            Message = "Item not found."
                        });
                    }

                    string sellerId = item.SellerID;

                    // Convert to review model and add the review
                    var reviewModel = model.ToModel(sellerId, buyerID);
                    var result = await reviewManager.Add(reviewModel);

                    if (result)
                    {
                        return new JsonResult(new ApiResultModel<bool>
                        {
                            result = true,
                            StatusCode = StatusCodes.Status200OK,
                            success = true,
                            Message = "Review added successfully."
                        });
                    }
                }
                return new JsonResult(new ApiResultModel<bool>
                {
                    result = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    success = false,
                    Message = "Failed to add review."
                });
            }

            return new JsonResult(new ApiResultModel<bool>
            {
                result = false,
                StatusCode = StatusCodes.Status400BadRequest,
                success = false,
                Message = "Invalid input."
            });
        }

        [HttpGet("GetSellerInfo/{id}")]
        public async Task<IActionResult> GetSellerInfo(int id)
        {

            try
            {
                var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == id).ToSellerInfo();

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

    }
}
