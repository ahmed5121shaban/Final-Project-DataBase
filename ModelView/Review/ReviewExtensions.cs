using System;
using System.IO;
using Final;
using Models;

namespace ModelView
{
    public static class ReviewExtensions
    {
        public static Review ToModel(this AddReviewViewModel model)
        {

            return new Review
            {
                Range = model.SellerRating,
                Description = model.SellerReview,
                SellerID = model.SellerID,
                BuyerID = model.BuyerID,
            };
        }

        public static AddReviewViewModel ToAddViewModel(this Review model)
        {
            return new AddReviewViewModel
            {
                SellerRating = model.Range,
                SellerReview = model.Description,
            };
        }
    }
}
