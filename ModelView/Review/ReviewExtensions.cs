using System;
using System.IO;
using Final;
using Models;

namespace ModelView
{
    public static class ReviewExtensions
    {
            public static Review ToModel(this AddReviewViewModel model, string sellerId, string buyerId)
            {
                return new Review
                {
                    Range = model.SellerRating,
                    Description = model.SellerReview,
                    SellerID = sellerId, // Set from method parameter
                    BuyerID = buyerId,   // Set from method parameter
                    AuctionID = model.AuctionID,
                };
            }

            public static AddReviewViewModel ToAddViewModel(this Review model)
            {
                return new AddReviewViewModel
                {
                    SellerRating = model.Range,
                    SellerReview = model.Description,
                    AuctionID = model.AuctionID,
                    
                };
            }

        public static ReviewViewModel ToViewModel(this Review model) {

            return new ReviewViewModel()
            {
                BuyerId =model.BuyerID,
                BuyerName = model.Buyer.User.Name,
                BuyerImage =model.Buyer.User.Image,
                ReviewRange = model.Range,
                review = model.Description,
               

            };


        }
        }
    }
