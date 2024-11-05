using System;
using Models.Models;

namespace ModelView
{
    public static class ComplainExtensions
    {
        // تحويل من ComplainAddViewModel إلى Complain
        public static Complain ToModel(this ComplainAddViewModel model)
        {
            return new Complain
            {
                Reason = model.Reason,
                SellerID = model.SellerID.ToString() // تحويل ID إلى string إن لزم الأمر
            };
        }

        // يمكن إضافة تحويلات أخرى إذا لزم الأمر لنماذج إضافية
    }
}
