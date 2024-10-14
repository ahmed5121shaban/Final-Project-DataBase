using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public static class BidExtensions
    {
        public static Bid ToModel(this AddBidViewModel model)
        {
            return new Bid
            {
                Amount=model.Amount,
                Time=(DateTime)model.Time,
                BuyerID =model.BuyerID,
                AuctionID=(int)model.AuctionID
            };
        }

    }
}
