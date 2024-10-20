using Final;
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
                // تحويل أرقام الهاتف إذا كانت موجودة
                PhoneNumbers = model.PhoneNumbers?.Select(phone => new PhoneNumber { Phone = phone }).ToList(),
                // يمكنك إضافة معالجة لصورة الملف هنا إذا كنت تريد تخزينها
                Image = model.ProfileImage != null ? ConvertToBase64(model.ProfileImage) : null
            };
        }

        // وظيفة لتحويل صورة الملف إلى Base64
        private static string ConvertToBase64(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
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
