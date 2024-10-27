using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class AddReviewViewModel
    {
     
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5.")]
        public byte SellerRating { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The review can't be longer than 1000 characters.")]
        public string SellerReview { get; set; }
        public DateTime ReviewDate { get; set; }
        public int AuctionID { get; set; }
    }
}
