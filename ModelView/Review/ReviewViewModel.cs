using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class ReviewPageViewModel
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, double> RatingPercentages { get; set; }
        public List<ReviewDetailViewModel> Reviews { get; set; }

        public string BuyerId {  get; set; }

        public string BuyerImage {  get; set; }

        public ReviewPageViewModel()
        {
            RatingPercentages = new Dictionary<int, double> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };
            Reviews = new List<ReviewDetailViewModel>();
        }
    }


    public class ReviewDetailViewModel
    {
        public string ReviewerID { get; set; }
        public string ReviewerName { get; set; }
        public string ProfileImageUrl { get; set; }
        public byte Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }


    }

}
