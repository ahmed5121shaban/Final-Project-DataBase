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
        [Required]
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5.")]
        public byte SellerRating { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The review can't be longer than 1000 characters.")]
        public string SellerReview { get; set; }

        [Required(ErrorMessage = "Seller ID is required.")]
        public string SellerID { get; set; }

        [Required(ErrorMessage = "Buyer ID is required.")]
        public string BuyerID { get; set; }

    }
}
