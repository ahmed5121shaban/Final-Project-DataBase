using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace ModelView
{
    public class BuyerProfileViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public int WonAuctions { get; set; }
        public int LostAuctions { get; set; }
        public decimal ProfileCompletion { get; set; }
        public List<AuctiondetailsViewModel> Auctions { get; set; } // تأكد من أن هذا هو النوع الصحيح
    }

}
