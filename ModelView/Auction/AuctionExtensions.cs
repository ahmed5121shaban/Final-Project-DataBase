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
        public static AuctiondetailsViewModel SeeDetails(this Auction auction)
        {
            return new AuctiondetailsViewModel
            {
                ID = auction.ID,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate,
                ItemID = auction.ItemID,
                Item = new AuctionItemViewModel
                {
                    ID = auction.Item.ID,
                    StartPrice = auction.Item.StartPrice,
                    status = auction.Item.Status,
                    Name = auction.Item.Name,
                    AddTime = auction.Item.AddTime,
                    Images = auction.Item.Images.Select(i=>i.Src).ToList(),
                    Description = auction.Item.Description,
                    CategoryID = auction.Item.CategoryID,
                    Category = auction.Item.Category.Name,
                    SellerId = auction.Item.SellerID,
                    SellerName = auction.Item.Seller.User.Name,

                },
                Bids = auction.Bids.Select(b=> new AutionBidViewModel { 
                    Amount = b.Amount,
                    BuyerID = b.BuyerID,
                    ID = b.ID,
                    BuyerName = b.Buyer.User.Name,
                    Time = b.Time,
                }).ToList(),
            };
        }
    }
}
