using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class AddBidViewModel
    {
        public int? ID { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Time { get; set; }
        public string? BuyerID { get; set; }
        public int AuctionID { get; set; }
    }
}
