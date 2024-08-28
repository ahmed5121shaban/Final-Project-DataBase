using Final.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zaeid.models
{
    public class Payment
    {
        public int Id { get; set; }
        public Enums.PaymentMetod Method {  get; set; }
        public bool IsDone { get; set; }
        public int UserId { get; set; }
        public int AuctionId {  get; set; }



    }
}
