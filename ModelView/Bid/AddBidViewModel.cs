using FinalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class AddBidViewModel
    {
        public int? ID { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public DateTime? Time { get; set; }
        public string? BuyerID { get; set; }
        [Required]
        public int? AuctionID { get; set; }
    }
}
