using FinalApi;
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

            var reviews = reviewManager.GetAll().Select(r=>r.ToViewModel()).ToList();
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
                    DateTime ReviewDate = DateTime.Now;
                    // Convert to review model and add the review
                    var reviewModel = model.ToModel(sellerId, buyerID, ReviewDate);
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

        [HttpGet("GetSellerReviews")]
        public async Task<IActionResult> GetSellerReviews()
        {
            var sellerID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sellerReviews = reviewManager.GetAll()
                                              .Where(r => r.SellerID == sellerID)
                                              .ToList();

            var reviewPageViewModel = new ReviewPageViewModel
            {
                TotalReviews = sellerReviews.Count,
                RatingPercentages = new Dictionary<int, double> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } }
            };

            if (sellerReviews.Any())
            {
                // Calculate average rating
                reviewPageViewModel.AverageRating = sellerReviews.Average(r => r.Range);

                // Calculate percentage breakdown for each rating
                for (int i = 1; i <= 5; i++)
                {
                    var count = sellerReviews.Count(r => r.Range == i);
                    reviewPageViewModel.RatingPercentages[i] = Math.Round((double)count / sellerReviews.Count * 100, 2);
                }


                // Map each review to ReviewDetailViewModel
                reviewPageViewModel.Reviews = sellerReviews.Select(r => new ReviewDetailViewModel
                {
                    ReviewerID = r.BuyerID,
                    ReviewerName = r.Buyer?.User?.Name ?? "Anonymous",
                    ProfileImageUrl = r.Buyer?.User?.Image ,
                    Rating = r.Range,
                    ReviewText = r.Description,
                    ReviewDate = r.Date,
                }).ToList();
            }

            return new JsonResult(reviewPageViewModel);
        }

    }
}
