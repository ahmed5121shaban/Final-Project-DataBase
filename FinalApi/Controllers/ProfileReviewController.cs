using Microsoft.AspNetCore.Mvc;
using FinalApi;
using Managers;
using ModelView;

namespace FinalApi.Controllers
{
    [Route("api/profile-review")]
    [ApiController]
    public class ProfileReviewController : ControllerBase
    {
        private readonly ProfileManager profileManager;

        public ProfileReviewController(ProfileManager profileManager)
        {
            this.profileManager = profileManager; // استخدم نفس اسم المتغير
        }

        [HttpGet("seller-profile/{userId}")]
        public async Task<ActionResult<SellerProfileViewModel>> GetSellerProfile(string userId)
        {
            var sellerProfile = await profileManager.GetSellerProfile(userId);
            if (sellerProfile == null)
                return NotFound();

            return Ok(sellerProfile);
        }

        [HttpGet("buyer-profile/{userId}")]
        public async Task<ActionResult<BuyerProfileViewModel>> GetBuyerProfile(string userId)
        {
            var buyerProfile = await profileManager.GetBuyerProfile(userId);
            if (buyerProfile == null)
                return NotFound();

            return Ok(buyerProfile);
        }

    }
}
