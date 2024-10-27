using FinalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView { 
    public class PaymentViewModel
    {
        public int? Id { get; set; }
        [Required]
        public Enums.PaymentMetod Method { get; set; }
        public string? BuyerId { get; set; }
        public int? AuctionID { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? PaypalEmail { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? StripeEmail { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get;set; }
        public string? Token { get; set; }
    }
}
