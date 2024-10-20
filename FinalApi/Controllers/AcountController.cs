﻿using Final;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView.Account;
using ModelView;
using System.Security.Claims;
using static Final.Enums;
using Microsoft.AspNet.Identity;

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

        public AcountController(AccountManager _acountManager,
            ReviewManager _reviewManager,
            AuctionManager _auctionManager,
            FavCategoryManager _favCategoryManager)
        {
            acountManager = _acountManager;
            this.auctionManager = auctionManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel _loginView)
        {
            if (!ModelState.IsValid) return BadRequest(new { Message = "This Data Is Not Completed" });
            var res = await acountManager.Login(_loginView);
            if (res == string.Empty) return BadRequest(new { Message = "Error In Login Operation" }); 
            return Ok(new {token=res,exepire=DateTime.Now.AddDays(30),status=200});
            if (res == null) return BadRequest(new { Message = "Error In Login Operation" });
            return Ok(new { token = res, exepire = DateTime.Now.AddDays(30), status = 200 });
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
                City = user.City,
                Country = user.Country,
                Street = user.Street,
                PostalCode = user.PostalCode,
                TimeZone = user.TimeZone,
                Currency = user.Currency,
                PhoneNumbers = user.PhoneNumbers.Select(p => p.Phone).ToList(),
                Age = user.Age.HasValue ? user.Age.Value : 0,
                NationalId = user.NationalId,
                Description = user.Description,
                Gender = user.Gender.HasValue ? user.Gender.Value : Final.Enums.Gender.male,
                Image = user.Image
            };

            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromForm] UpdateProfileViewModel model)
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


            if (model.ProfileImage != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, model.ProfileImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.Image = filePath;
            }

            var existingUser = await acountManager.UserManager.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return BadRequest("This email is already taken.");
            }

            var result = await acountManager.UpdateUserProfileAsync(user, model);

            if (result.Succeeded)
            {
                // بعد التحديث، يتم تعيين البريد الإلكتروني كاسم المستخدم
                user.UserName = user.Email;

                var updateResult = await acountManager.UserManager.UpdateAsync(user); // تحديث اسم المستخدم

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

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFrontFileName = Guid.NewGuid().ToString() + frontExtension;
            var uniqueBackFileName = Guid.NewGuid().ToString() + backExtension;

            var nationalIdFrontPath = Path.Combine(uploadsFolder, uniqueFrontFileName);
            var nationalIdBackPath = Path.Combine(uploadsFolder, uniqueBackFileName);

            try
            {
                using (var stream = new FileStream(nationalIdFrontPath, FileMode.Create))
                {
                    await model.NationalIdFrontImage.CopyToAsync(stream);
                }

                using (var stream = new FileStream(nationalIdBackPath, FileMode.Create))
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
                System.IO.File.Delete(nationalIdFrontPath);
                System.IO.File.Delete(nationalIdBackPath);
                return NotFound("User not found.");
            }

            // هنا يتم التحديث مباشرة بدون UpdateProfileViewModel
            user.NationalId = (model.IdNumber);
            user.NationalIdFrontImage = nationalIdFrontPath; // افترض أن الحقل موجود في كلاس المستخدم
            user.NationalIdBackImage = nationalIdBackPath;
            user.BarthDate = model.BarthDate; // تأكد من أن هذا الحقل موجود في المستخدم

            var result = await acountManager.UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                System.IO.File.Delete(nationalIdFrontPath);
                System.IO.File.Delete(nationalIdBackPath);
                return BadRequest(new { Message = "Error updating user information", Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { message = "Identity verification is successful", data = model, FormattedBirthDate = user.BarthDate.ToString("dd/MM/yyyy") });
        }

        private const long MaxImageSizeInBytes = 5 * 1024 * 1024; // 5MB as an example

        private IFormFile ConvertBase64ToImage(string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
                return null;

            string base64Data;

            // Check if there is a comma in the base64 string (metadata prefix)
            if (base64Image.Contains(","))
            {
                base64Data = base64Image.Split(',')[1]; // Extract actual base64 data
            }
            else
            {
                base64Data = base64Image; // No metadata, treat it as the base64 string itself
            }

            // Check if the remaining string is a valid Base64 string
            if (string.IsNullOrEmpty(base64Data) || !IsBase64String(base64Data))
            {
                throw new FormatException("The provided string is not a valid Base64 encoded image.");
            }

            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Check if the image size is too large
            if (imageBytes.Length > MaxImageSizeInBytes)
            {
                throw new InvalidOperationException("Image size is too large.");
            }

            // Create a memory stream from the byte array
            using (var ms = new MemoryStream(imageBytes))
            {
                var fileName = "image.png"; // or any other name you want to give
                var file = new FormFile(ms, 0, imageBytes.Length, fileName, fileName)
                {
                    ContentType = "image/png" // Change according to your image type
                };
                return file;
            }
        }

        private bool IsBase64String(string base64String)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64String.Length]);
            return Convert.TryFromBase64String(base64String, buffer, out _);
        }

        [HttpGet("UserProfile/{UserId}")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(string UserId)
        {
            var user = await acountManager.UserManager.FindByIdAsync(UserId);
            var role = await acountManager.UserManager.GetRolesAsync(user);
            bool isseller = (role.Contains("Seller")) ? true : false ;
            var sellerRates = reviewManager.GetAll().Where(r => r.SellerID == UserId).Select(r => r.Range).ToList();
            int range = 0;
            foreach (var sellerRate in sellerRates)
            {
                range += sellerRate;
            }
            decimal finalRate = range / sellerRates.Count;

            return Ok( new ProfileViewModel()
            {
                FullName = user.Name,
                IsSeller = isseller,
                Rate = finalRate,
                AuctionsNumber = auctionManager.GetAll().Where(a => a.Item.SellerID == UserId).Count(),
                Address = $"{user.City} ,{user.Country}",
                ReviewsNumber = reviewManager.GetAll().Where(r => r.SellerID == UserId).Count(),
                FavCategories = favCategoryManager.GetAll().Where(f => f.BuyerID == UserId).Select(f => f.Category).ToList(),
                LatestAuctions = auctionManager.GetAll().Where(a => a.Item.SellerID == UserId).OrderByDescending(a => a.StartDate).Skip(0).Take(10).ToList(),
                WonAuctions = auctionManager.GetAll().Where(a=>a.BuyerID == UserId).ToList(),
                reviews = reviewManager.GetAll().Where(r=>r.SellerID ==UserId).ToList()

            });

            
        }
    }
}
