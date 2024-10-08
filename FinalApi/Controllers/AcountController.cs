using Final.ViewModels;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using ModelView.Account;
using System.Security.Claims;
using static Final.Enums;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        private readonly AccountManager acountManager;

        public AcountController(AccountManager _acountManager)
        {
            acountManager = _acountManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel _loginView)
        {
            if (!ModelState.IsValid) return BadRequest(new { Message = "This Data Is Not Completed" });
            var res = await acountManager.Login(_loginView);
            if (res == null) return BadRequest(new { Message = "Error In Login Operation" });
            return Ok(new { token = res, exepire = DateTime.Now.AddDays(30), status = 200 });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel _registerView)
        {
            try
            {

                if (!ModelState.IsValid) return BadRequest(new { Message = "This Data Is Not Completed" });
                var res = await acountManager.Register(_registerView);
                if (!res.Succeeded) return BadRequest(new { Message = "Error In Register Operation" });
                return Ok(new { status = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error In Register Operation" });
            }
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
                PhoneNumbers = user.PhoneNumbers.Select(p => p.Phone).ToList(),
                Age = user.Age.HasValue ? user.Age.Value : 0,
                NationalId = user.NationalId.HasValue ? user.NationalId.Value : 0,
                Description = user.Description,
                Gender = user.Gender.HasValue ? user.Gender.Value : Final.Enums.Gender.male,
                Image = user.Image
            };

            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromForm] UpdateProfileViewModel model,
            [FromForm] IFormFile profileImage)
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

            if (profileImage != null && profileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, profileImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.Image = filePath;
            }

            var result = await acountManager.UpdateUserProfileAsync(user, model);

            if (result.Succeeded)
            {
                return Ok("Profile updated successfully.");
            }
            else
            {
                return BadRequest(new { Message = "Error updating profile", Errors = result.Errors.Select(e => e.Description) });
            }
        }
        [Authorize]
        [HttpPost]
        [Route("VerifyIdentity")]
        public async Task<IActionResult> VerifyIdentity([FromForm] VerifyIdentityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // التأكد من أن الملف موجود وأنه ليس فارغًا
            if (model.IdDocument != null && model.IdDocument.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // التأكد من وجود مجلد الرفع، إذا لم يكن موجودًا يتم إنشاؤه
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // إنشاء مسار الملف وحفظه
                var filePath = Path.Combine(uploadsFolder, model.IdDocument.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.IdDocument.CopyToAsync(stream);
                }

                // إذا كنت بحاجة إلى حفظ مسار الملف المحفوظ في مكان آخر، يمكنك تخزينه في قاعدة البيانات
            }
            else
            {
                return BadRequest("ID document is required.");
            }

            // متابعة العملية بعد رفع الملف، مثل حفظ باقي البيانات في قاعدة البيانات

            return Ok(new { message = "Identity verification is successful", data = model });
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
    }

}

    