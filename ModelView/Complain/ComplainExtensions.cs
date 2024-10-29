using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models; // تأكد من وجود الـ using المناسب

namespace ModelView
{
    public static class ComplainExtensions
    {
        // إذا كانت Models.Models.Complain هي كلاس، تأكد من استخدامه بالطريقة الصحيحة.
        public static Models.Models.Complain ToModel(this ComplainAddViewModel model)
        {
            return new Models.Models.Complain() // استخدام المساحة اسم كاملة لتجنب التداخل
            {
                Reason = model.Reason,
                BuyerID = model.BuyerID,
                SellerID = model.SellerID
            };
        }

        // يمكنك إضافة تحويلات إضافية إذا كان لديك نماذج أخرى مشابهة
    }
}
