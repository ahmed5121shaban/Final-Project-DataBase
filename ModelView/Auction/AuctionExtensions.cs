using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
     public static class AuctionExtensions
    {
        public static Auction toAuctionModel(this AddAuctionModel model)
        {
            return new Auction
            {
                StartDate = model.StartDate,
                EndDate = model.StartDate.AddDays(model.Duration),
                ItemID = model.ItemId,
                Completed =false
            };
        }
    }
}
