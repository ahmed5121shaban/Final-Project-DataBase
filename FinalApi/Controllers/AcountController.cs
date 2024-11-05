using FinalApi;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView.Account;
using ModelView;
using System.Security.Claims;
using static FinalApi.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Caching.Memory;


namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        private readonly AccountManager acountManager;
        private readonly ReviewManager reviewManager;
        private readonly AuctionManager auctionManager;
        private readonly FavCategoryManager favCategoryManager;
        private readonly CloudinaryManager cloudinaryManager;
        private readonly IMemoryCache memoryCache;

        public AcountController(AccountManager _acountManager,
            ReviewManager _reviewManager,
            AuctionManager _auctionManager,
            FavCategoryManager _favCategoryManager,
            CloudinaryManager _cloudinaryManager,IMemoryCache _memoryCache)
        {
            acountManager = _acountManager;
           auctionManager = _auctionManager;
            reviewManager = _reviewManager;
            favCategoryManager = _favCategoryManager;
            cloudinaryManager = _cloudinaryManager;
            memoryCache = _memoryCache;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel _loginView)
        {
            if (!ModelState.IsValid) return BadRequest(new { Message = "This Data Is Not Completed" });
            var res = await acountManager.Login(_loginView);
            if (res == string.Empty) return BadRequest(new { Message = "Error In Login Operation" }); 
            return Ok(new {token=res,exepire=DateTime.Now.AddDays(30),status=200});
         
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel _registerView)
        {
            if (!ModelState.IsValid) return BadRequest(new { Message = "This Data Is Not Completed" });
            var res = await acountManager.Register(_registerView);
            if (!res.Succeeded) return BadRequest(new { Message = "Error In Register Operation" });
            return Ok(new { status = 200 });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User not found.");
            }

            if(memoryCache.TryGetValue($"profile{userId}", out var userCache))
            {
                return Ok(userCache);
            }


            var user = await acountManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var profile = new UpdateProfileViewModel
            {
                FirstName = user.Name,
                LastName = user.Name,
                Email = user.Email,
                TimeZone = user.TimeZone,
                Currency = user.Currency,
                PhoneNumbers = user.PhoneNumbers.Select(p => p.Phone).ToList(),
                Age = user.Age.HasValue ? user.Age.Value : 0,
                Description = user.Description,
                Gender = user.Gender.HasValue ? user.Gender.Value : FinalApi.Enums.Gender.male,
                Image = user.Image
            };
            memoryCache.Set($"profile{userId}", profile);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await acountManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if a profile image was provided, otherwise use the default image
            if (model.ProfileImage != null)
            {
                user.Image =await cloudinaryManager.UploadFileAsync(model.ProfileImage);
            }
         
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var existingUser = await acountManager.UserManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest("This email is already taken.");
                }
                user.Email = model.Email;
            }

            if (!string.IsNullOrWhiteSpace(model.FirstName) || !string.IsNullOrWhiteSpace(model.LastName))
            {
                user.Name = $"{model.FirstName} {model.LastName}".Trim();
            }

            if (model.Age > 0)
            {
                user.Age = model.Age;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                user.Description = model.Description;
            }

            if (model.Gender != null)
            {
                user.Gender = model.Gender;
            }

            if (model.PhoneNumbers != null && model.PhoneNumbers.Any())
            {
                user.PhoneNumbers = model.PhoneNumbers
                    .Where(phone => !string.IsNullOrWhiteSpace(phone))
                    .Select(phone => new PhoneNumber { Phone = phone })
                    .ToList();
            }

            var result = await acountManager.UpdateUserProfileAsync(user, model);

            if (result.Succeeded)
            {
                user.UserName = user.Email;

                var updateResult = await acountManager.UserManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    return Ok("Profile updated successfully.");
                }
                else
                {
                    return BadRequest(new { Message = "Error updating username", Errors = updateResult.Errors.Select(e => e.Description) });
                }
            }
            else
            {
                return BadRequest(new { Message = "Error updating profile", Errors = result.Errors.Select(e => e.Description) });
            }
        }

        [Authorize]
        [HttpPut("profile/address")]
        public async Task<IActionResult> UpdateAddress([FromForm] UpdateAddressViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await acountManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!string.IsNullOrWhiteSpace(model.City))
            {
                user.City = model.City;
            }

            if (!string.IsNullOrWhiteSpace(model.Country))
            {
                user.Country = model.Country;
            }

            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                user.Street = model.Street;
            }

            if (!string.IsNullOrWhiteSpace(model.PostalCode))
            {
                user.PostalCode = model.PostalCode;
            }

            var result = await acountManager.UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Address updated successfully.");
            }
            else
            {
                return BadRequest(new { Message = "Error updating address", Errors = result.Errors.Select(e => e.Description) });
            }
        }

        [Authorize]
        [HttpPost("VerifyIdentity")]
        public async Task<IActionResult> VerifyIdentity([FromForm] VerifyIdentityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var frontExtension = Path.GetExtension(model.NationalIdFrontImage.FileName).ToLower();
            var backExtension = Path.GetExtension(model.NationalIdBackImage.FileName).ToLower();

            if (!allowedExtensions.Contains(frontExtension) || !allowedExtensions.Contains(backExtension))
            {
                return BadRequest("Only .jpg, .jpeg, and .png files are allowed.");
            }

            var nationalIdImagesFolder = Path.Combine("Images", "national_id_images");
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", nationalIdImagesFolder));

            var uniqueFrontFileName = $"{Guid.NewGuid()}{frontExtension}";
            var uniqueBackFileName = $"{Guid.NewGuid()}{backExtension}";

            var nationalIdFrontPath = Path.Combine(nationalIdImagesFolder, uniqueFrontFileName);
            var nationalIdBackPath = Path.Combine(nationalIdImagesFolder, uniqueBackFileName);

            var absoluteFrontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", nationalIdFrontPath);
            var absoluteBackPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", nationalIdBackPath);

            try
            {
                using (var stream = new FileStream(absoluteFrontPath, FileMode.Create))
                {
                    await model.NationalIdFrontImage.CopyToAsync(stream);
                }

                using (var stream = new FileStream(absoluteBackPath, FileMode.Create))
                {
                    await model.NationalIdBackImage.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"File upload failed: {ex.Message}");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await acountManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                System.IO.File.Delete(absoluteFrontPath);
                System.IO.File.Delete(absoluteBackPath);
                return NotFound("User not found.");
            }

            user.NationalId = model.IdNumber;
            user.NationalIdFrontImage = nationalIdFrontPath; // المسار النسبي لصورة الجهة الأمامية
            user.NationalIdBackImage = nationalIdBackPath;   // المسار النسبي لصورة الجهة الخلفية
            user.BarthDate = model.BarthDate;

            var result = await acountManager.UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                System.IO.File.Delete(absoluteFrontPath);
                System.IO.File.Delete(absoluteBackPath);
                return BadRequest(new { Message = "Error updating user information", Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { message = "Identity verification is successful", data = model, FormattedBirthDate = user.BarthDate.ToString("dd/MM/yyyy") });
        }

      
        [HttpGet("UserProfile/{UserId}")]
        public async Task<IActionResult> GetUserProfile(string UserId)
        {
            var user = await acountManager.UserManager.FindByIdAsync(UserId);
            var role = await acountManager.UserManager.GetRolesAsync(user);
            bool isseller = (role.Contains("Seller")) ? true : false ;
            var sellerRates = reviewManager.GetAll().Where(r => r.SellerID == UserId).Select(r => r.Range).ToList();
            int range = 0;
            decimal finalRate = 0;
            if (sellerRates.Count > 0)
            {
                foreach (var sellerRate in sellerRates)
                {
                    range += sellerRate;
                }
                finalRate = range / sellerRates.Count;
            }
            return Ok(new ProfileViewModel()
            {
                FullName = user.Name,
                Image=user.Image,
                IsSeller = isseller,
                Rate = finalRate,
                AuctionsNumber = auctionManager.GetAll().Where(a => a.Item.SellerID == UserId).Count(),
                Address = $"{user.City} ,{user.Country}",
                ReviewsNumber = reviewManager.GetAll().Where(r => r.SellerID == UserId).Count(),
                FavCategories = favCategoryManager.GetAll().Where(f => f.BuyerID == UserId).Select(f => f.Category.ToProfileCatViewModel()).ToList(),
                LatestAuctions = auctionManager.GetAll().Where(a => a.Item.SellerID == UserId).OrderByDescending(a => a.StartDate).Skip(0).Take(10).Select(i=>i.SeeDetails()).ToList(),
                WonAuctions = auctionManager.GetAll().Where(a => a.BuyerID == UserId).Select(i=>i.SeeDetails()).ToList(),
                Reviews = reviewManager.GetAll().Where(r => r.SellerID == UserId).Select(i => i.ToViewModel()).ToList()

            });

            
        }

        [Authorize]
        [HttpGet("UserData")]
        public async Task<IActionResult> GetUserData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await acountManager.UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userdata = new UserDataViewModel()
            {
                Name = user.Name,
                Image = user.Image
            };

            return Ok(userdata);
        }


        [Authorize]
        [HttpGet("userCurrency")]
        public async Task<IActionResult> GetUserCurrency()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memoryCache.TryGetValue($"userCurrency-{userId}", out var resultCache))
                return Ok(resultCache);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is null or empty.");
            }

            var user = await acountManager.UserManager.FindByIdAsync(userId);
            var userCurrency = user.Currency ;

            memoryCache.Set($"userCurrency-{userId}", new ApiResultModel<string>()
            {
                result = userCurrency
            });

            return Ok(new ApiResultModel<string>()
            {
                result = userCurrency
            });
        }

    }
}
