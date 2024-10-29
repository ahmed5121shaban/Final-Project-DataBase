using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace ModelView
{
    public class SellerProfileViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public int CompletedAuctions { get; set; }
        public int UnfinishedAuctions { get; set; }
        public decimal SellerRating { get; set; }
        public decimal ProfileCompletion { get; set; }
        public List<ItemViewModel> Items { get; set; } // إضافة هنا لجلب العناصر الخاصة بالبائع
    }




}
