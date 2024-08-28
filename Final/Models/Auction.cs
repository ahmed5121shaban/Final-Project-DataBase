using finalproject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zaeid.models;

namespace Final.Models
{
    public class Auction
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ItemID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PaymentID { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<User> Users { get; set; }



    }

    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.HasOne(p => p.Payment).WithOne(u => u.Auction).HasForeignKey<Auction>(p => p.PaymentID);
        }
    }
}
