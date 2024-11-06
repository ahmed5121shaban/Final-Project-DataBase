using FinalApi;
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

        public static BidViewModel ToBidViewModel(this Bid _bid)
        {
            return new BidViewModel
            {
                BidAmount = _bid.Amount,
                BidTime = _bid.Time,
                BuyerName = _bid?.Buyer?.User?.Name??"1",
            };
        }
    }

    }

