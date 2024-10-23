using Microsoft.AspNetCore.Mvc;
using Final;
using Managers;
using ModelView.Profile;
using ModelView;
using Microsoft.AspNet.Identity;


namespace FinalApi.Controllers
{
 
    [Route("api/profile-review")]
    [ApiController]
    public class ProfileReviewController : ControllerBase
    {
        private readonly ProfileManager _profileManager;

        public ProfileReviewController(ProfileManager profileManager)
        {
            _profileManager = profileManager;
        }

        [HttpGet("seller-profile/{userId}")]
        public ActionResult<SellerProfileViewModel> GetSellerProfile(int userId)
        {
            var sellerProfile = _profileManager.GetSellerProfile(userId);
            if (sellerProfile == null)
                return NotFound();

            return Ok(sellerProfile);
        }

        [HttpGet("buyer-profile/{userId}")]
        public ActionResult<BuyerProfileViewModel> GetBuyerProfile(int userId)
        {
            var buyerProfile = _profileManager.GetBuyerProfile(userId);
            if (buyerProfile == null)
                return NotFound();

            return Ok(buyerProfile);
        }
    }
}
