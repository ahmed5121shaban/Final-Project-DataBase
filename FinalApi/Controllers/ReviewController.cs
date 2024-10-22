using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private ReviewManager reviewManager;
       public ReviewController(ReviewManager _reviewManager) {
        this.reviewManager = _reviewManager;
        
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
                var result = await reviewManager.Add(model.ToModel()); 
                if (result)
                {
                    return Ok(new { message = "Review added successfully" });
                }
                return BadRequest(new { message = "Failed to add review" });
            }

         
            return BadRequest(ModelState);
        }
    
}
}
