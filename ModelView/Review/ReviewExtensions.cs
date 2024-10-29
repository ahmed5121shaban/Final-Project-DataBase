using System;
using System.IO;
using FinalApi;
using Models;

namespace ModelView
{
    public static class ReviewExtensions
    {
            public static Review ToModel(this AddReviewViewModel model, string sellerId, string buyerId , DateTime ReviewDate)
            {
                return new Review
                {
                    Range = model.SellerRating,
                    Description = model.SellerReview,
                    SellerID = sellerId, // Set from method parameter
                    BuyerID = buyerId,   // Set from method parameter
                    AuctionID = model.AuctionID,
                    Date=model.ReviewDate
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


        public static ReviewDetailViewModel ToViewModel(this Review model)
        {

            return new ReviewDetailViewModel()
            {
                ReviewerID = model.BuyerID,
                ReviewerName = model.Buyer.User.Name,
                ProfileImageUrl = model.Buyer.User.Image,
                Rating = model.Range,
                ReviewText = model.Description,
                ReviewDate = model.Date


            };


        }
    }
    }