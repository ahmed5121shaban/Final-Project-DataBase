using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class LostAuctionViewModel
    {
        public string AuctionTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<LostBidsViewModel> Bids  { get; set; }
    }
    public class LostBidsViewModel
    {
        public decimal BidAmount {  get; set; }
        public string BuyerName { get; set; }
    }

}
