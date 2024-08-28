using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class Chat
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public DateTime StartDate { get; set; }

        public virtual User Buyer { get; set; }
        public virtual User Seller { get; set; }
        public virtual ICollection<Message> ChatMessages { get; set; }

    }
}
