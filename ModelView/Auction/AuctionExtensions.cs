using FinalApi;
using ModelView;
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
                StartDate = model.StartDate.ToUniversalTime(),
                EndDate = model.StartDate.AddMinutes(model.Duration).ToUniversalTime(),
                ItemID = model.ItemId,
                Completed =false
            };
        }
        public static AuctiondetailsViewModel SeeDetails(this Auction auction)
        {
            decimal startPrice = auction.Item.StartPrice;
            decimal totalBids = 0;
            foreach (var bid in auction.Bids)
            {
                totalBids += bid.Amount;
            }
            var totalPrice = startPrice + totalBids;
            return new AuctiondetailsViewModel
            {
                ID = auction.ID,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate.AddHours(2d),
                ItemID = auction.ItemID,
                IsEnded = auction.Ended,
                Completed=auction.Completed,
                currentPrice=totalPrice,
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

        public static DoneAuctionViewModel ToDoneAuctionVM(this Auction _auction)
        {
            decimal startPrice = _auction.Item.StartPrice;
            decimal totalBids = 0;
            foreach (var bid in _auction.Bids)
            {
                totalBids += bid.Amount;
            }
            var totalPrice = startPrice + totalBids;
            return new DoneAuctionViewModel
            {
                AuctionTitle = _auction.Item.Name,
                EndDate = _auction.EndDate,
                StartDate= _auction.StartDate,
                Completed= _auction.Completed,
                ShippingStatus = _auction.ShippingStatus,
                AuctionID = _auction.ID,
                ItemID = _auction.Item.ID,
                SellerName = _auction.Item.Seller.User.Name,
                ImageUrl = _auction.Item.Images.Select(i=>i.Src).ToList(),
                TotalPrice=totalPrice
            };
        }

        public static CompletedAuctionViewModel ToCompletedAuctionVM(this Auction _auction)
        {
            decimal startPrice = _auction.Item.StartPrice;
            decimal totalBids = 0;
            foreach(var bid in _auction.Bids)
            {
                totalBids += bid.Amount;
            }
            var totalPrice = startPrice + totalBids;
            return new CompletedAuctionViewModel
            {
                Id=_auction.ID,
                Name = _auction.Item.Name,
                EndDate = _auction.EndDate,
                item = new AuctionItemViewModel
                {
                    ID = _auction.Item.ID,
                    StartPrice = _auction.Item.StartPrice,
                    status = _auction.Item.Status,
                    Name = _auction.Item.Name,
                    AddTime = _auction.Item.AddTime,
                    Images = _auction.Item.Images.Select(i => i.Src).ToList(),
                    Description = _auction.Item.Description,
                    CategoryID = _auction.Item.CategoryID,
                    Category = _auction.Item.Category.Name,
                    SellerId = _auction.Item.SellerID,
                    SellerName = _auction.Item.Seller.User.Name,

                },
                totalPrice = totalPrice,
                BuyerName = _auction.Buyer?.User?.Name,
                BidsNumber = _auction.Bids.Count()
            };
        }

        public static LostAuctionViewModel ToLostAuctionVM(this Auction _auction)
        {
            List<LostBidsViewModel> bidsList = new List< LostBidsViewModel >();
            foreach (var item in _auction.Bids)
            {
                bidsList.Add(new LostBidsViewModel 
                {
                    BidAmount = item.Amount,
                    BuyerName = item.Buyer.User.Name,
                }
                );
            }
            return new LostAuctionViewModel
            {
                AuctionTitle = _auction.Item.Name,
                EndDate = _auction.EndDate,
                StartDate = _auction.StartDate,
                Bids = bidsList,
            };
        }

        public static CompleteAuctionPaymentViewModel ToCompleteAuctionPayment(this Item _item,decimal _bidsAmount)
        {
            var method = _item.Auction.Payment?.Method;
            return new CompleteAuctionPaymentViewModel
            {
                ID = (int)_item.AuctionID,
                City = _item.Seller.User?.City ?? "no City",
                Country = _item.Seller.User?.Country ?? "no Country",
                Street = _item.Seller.User?.Street ?? "no Street",
                CreationDate = _item.Auction.StartDate,
                ItemDescription = _item.Description,
                ItemName = _item.Name,
                PhoneNumber = _item.Seller.User?.PhoneNumber ?? "no PhoneNumber",
                StartPrice = _item.StartPrice,
                SellerName = _item.Seller.User.Name,
                Status = "UnPaid",
                Taxes = 5,
                EndPrice = _item.EndPrice,
                TotalBids = _bidsAmount,
                Method = method ?? 0,
                Currency = "USD",
                Images = _item.Images.Select(i => i.Src).ToList()
            };
        }

        public static AuctionSellerInfoViewModel ToSellerInfo(this Auction auction)
        {
            return new AuctionSellerInfoViewModel
            {
                AuctionTitle = auction.Item.Name,
                SellerName = auction.Item.Seller.User.Name,
                SellerImage = auction.Item.Seller.User.Image,
            };
        }

    
    
    }
}
