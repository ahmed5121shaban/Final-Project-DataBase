using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class DoneAuctionViewModel
    {
        public int ItemID { get; set; }
        public string AuctionTitle { get; set; }
        public string SellerName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Completed { get; set; }
        public List<string> ImageUrl { get; set; }
    }
}
