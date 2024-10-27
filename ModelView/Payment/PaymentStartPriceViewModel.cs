using FinalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class PaymentStartPriceViewModel
    {
        
        public string? BuyerID { get; set; }
        [Required]
        public int AuctionID { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? PayPalEmail { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? StripeEmail { get; set; }
        [Required]
        public Enums.PaymentMetod Method { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string? Currency {  get; set; }

    }
}
