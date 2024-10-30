using FinalApi;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ModelView.Account
{
    public static class AccountExtension
    {
        public static User ToModel(this RegisterViewModel model)
        {
            return new User
            {
                UserName = model.Email,
                Name = $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                Image = "Images/profile_images/blank-profile-picture-973460_1920.png" // الصورة الافتراضية

            };
            }

        public static User ToModel(this UpdateProfileViewModel model)
        {
            return new User
            {
                UserName = $"{model.FirstName}{model.LastName}", // تعديل لتوليد اسم المستخدم بناءً على الاسم الأول والأخير
                Name = $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                City = model.City,
                Country = model.Country,
                Street = model.Street,
                PostalCode = model.PostalCode,
                TimeZone = model.TimeZone,
                Currency = model.Currency,
                Age = model.Age,
                Description = model.Description,
                Gender = model.Gender,
                PhoneNumbers = model.PhoneNumbers?.Select(phone => new PhoneNumber { Phone = phone }).ToList(),
                // تخزين مسار الصورة النسبي بدلاً من تحويلها إلى Base64
                Image = model.ProfileImage != null ? SaveProfileImage(model.ProfileImage) : null
            };
        }

        // وظيفة لحفظ صورة الملف وإرجاع المسار النسبي
        private static string SaveProfileImage(IFormFile file)
        {
            // تحديد مسار الحفظ النسبي
            var relativePath = Path.Combine("uploads", "profile_images", $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
            var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            // التأكد من أن المجلد موجود
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            // حفظ الملف في المسار المحدد
            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return relativePath; // إرجاع المسار النسبي
        }


        public static User ToModel(this VerifyIdentityViewModel model)
        {
            return new User
            {
                // يتم هنا تحويل بيانات التحقق إلى خصائص المستخدم
                Name = $"{model.FirstName} {model.LastName}",
                BarthDate = model.BarthDate,
                NationalId = (model.IdNumber), // تحويل IdNumber إلى NationalId كـ int
                // يمكنك إضافة خصائص أخرى بناءً على النموذج
            };
        }
        //public static User ToModel(this LoginViewModel model) 
        //{
        //    return new User 
        //    {
        //       Email = model.Email,

        //    };
        //}


    }
}
