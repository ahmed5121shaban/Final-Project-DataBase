using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private ReviewManager reviewMnager;
       public ReviewController(ReviewManager _reviewManager) {
        this.reviewMnager=_reviewManager;
        
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetReviews()
        {

            var reviews = reviewMnager.GetAll().ToList();
            return new JsonResult(reviews);
        }

    }
}
