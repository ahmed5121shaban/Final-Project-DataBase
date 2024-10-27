using FinalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class AddPaymentViewModel
    {
        public int? Id { get; set; }
        [Required]
        public Enums.PaymentMetod Method { get; set; }
        
        public string? BuyerId { get; set; }
        
        public int? AuctionID { get; set; }
    }
}
