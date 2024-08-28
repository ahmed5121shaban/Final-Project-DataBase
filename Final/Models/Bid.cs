using Final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finalproject.Models
{
    public class Bid
    {
        public int ID { get; set; }
        public  decimal Amount { get; set; }    

        public DateTime Time { get; set; }

        public int Auction_Id { get; set; }    

        public int UserId { get; set; }
        public virtual User User { get; set; }

    }
}
