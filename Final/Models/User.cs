using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BarthDate { get; set; }
        public int Rate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string Gender { get; set; }
        public string Address { get; set; }
        public int NationalId { get; set; }
        public string TimeZone { get; set; }

        public string Description { get; set; }
        public int PhoneNosID { get; set; }
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
        //public virtual ICollection<Item> Items { get; set; }
        //public virtual ICollection<Chat> Chats { get;set; }

        //public virtual ICollection<SavedItem> SavedItems { get; set; }
        //public virtual ICollection<Bid> Bids { get; set; }

        //public virtual ICollection<Notifacation> Notifacations { get; set; }

        //public virtual ICollection<Auction> Auctions { get; set; }



    }


}
