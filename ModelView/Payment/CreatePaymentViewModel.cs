using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace ModelView
{
    public class CreatePaymentViewModel
    {
        public string Currency {  get; set; }
        [Required]
        public decimal Amount { get; set; }

        public int? auctionID { get; set; }
    }
}
