using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class CompletedAuctionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BuyerName { get; set; }
        public AuctionItemViewModel item { get; set; }
        public decimal totalPrice { get; set; }
        public DateTime EndDate { get; set; }

        public int BidsNumber { get; set; }
    }
}
