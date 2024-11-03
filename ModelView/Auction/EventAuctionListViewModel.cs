using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class EventItemsListViewModel
    {
        public int AuctionId { get; set; }
        public string Name { get; set; }
        public int BidCount { get; set; }
        public decimal StartPrice { get; set; }
        public List<string> Images { get; set; }
        public string SellerName { get; set; }
    }
}
