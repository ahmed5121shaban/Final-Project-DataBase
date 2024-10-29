using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace ModelView
{
    public static class ProfileExtension
    {
        // تحويل SellerProfileViewModel إلى User
        public static User ToUserModel(this SellerProfileViewModel model)
        {
            return new User
            {
                Id = model.UserId,
                Name = model.Name,
                Email = model.Email,
                Image = model.ImageUrl,
                // يمكنك إضافة خصائص أخرى هنا مثل Address أو Gender بناءً على الـ ViewModel
            };
        }

        // تحويل BuyerProfileViewModel إلى User
        public static User ToUserModel(this BuyerProfileViewModel model)
        {
            return new User
            {
                Id = model.UserId,
                Name = model.Name,
                Email = model.Email,
                Image = model.ImageUrl,
                // إضافة الخصائص الأخرى إذا لزم الأمر
            };
        }
    }
}
