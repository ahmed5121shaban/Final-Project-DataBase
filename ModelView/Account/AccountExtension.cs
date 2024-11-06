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
                Image = "https://res.cloudinary.com/dmpijaisv/image/upload/v1730921960/obvfkgrh1fsnfwftomej.png" // الصورة الافتراضية
            };
            }

        public static User ToModel(this UpdateProfileViewModel model)
        {
            return new User
            {
                UserName = $"{model.FirstName}{model.LastName}", // تعديل لتوليد اسم المستخدم بناءً على الاسم الأول والأخير
                Name = $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                TimeZone = model.TimeZone,
                Currency = model.Currency,
                Age = model.Age,
                Description = model.Description,
                Gender = model.Gender,
                PhoneNumbers = model.PhoneNumbers?.Select(phone => new PhoneNumber { Phone = phone }).ToList(),
                // تخزين مسار الصورة النسبي بدلاً من تحويلها إلى Base64
                Image = model.Image,
            };
        }


        public static User ToModel(this UpdateAddressViewModel model)
        {
            return new User
            {
                City = model.City,
                Country = model.Country,
                Street = model.Street,
                PostalCode = model.PostalCode,
            };
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

    }
}
