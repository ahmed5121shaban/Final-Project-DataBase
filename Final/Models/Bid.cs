using FinalApi;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalApi
{
    public class Bid
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }    
        public DateTime Time { get; set; }
        //public string PaymentEmail { get; set; }
        public string BuyerID { get; set; }
        public virtual Buyer Buyer { get; set; }
        public int AuctionID { get; set; }
        public virtual Auction Auction { get; set; }
    }

    public class BidConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            //buyer add some Bids
            builder.HasOne(b=>b.Buyer).WithMany(u=>u.Bids).HasForeignKey(b=>b.BuyerID);
            builder.HasOne(b => b.Auction).WithMany(a => a.Bids).HasForeignKey(b => b.AuctionID);
            builder.Property(b => b.Amount).IsRequired();
            builder.Property(b => b.Time).IsRequired();

        }
    }
}
