using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;

using static FinalApi.Enums;
using FinalApi;

namespace FinalApi
{
    public class Auction
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ItemID { get; set; }
        public virtual Item Item { get; set; }
        public int? PaymentID { get; set; }
        public virtual Payment Payment { get; set; }
        public string? BuyerID { get; set; }
        public virtual Buyer Buyer { get; set; }
        public virtual ICollection<FavAuctions> FavAuctions { get; set; }
        public virtual Review Review { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public bool Completed { get; set; }
        public bool Ended { get; set; }
        public AuctionShippingStatus ShippingStatus { get; set; }
    }

    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.HasKey(a => a.ID);
            builder.Property(a => a.StartDate).IsRequired();
            builder.Property(a => a.EndDate).IsRequired();
            builder.Property(a => a.Ended).HasDefaultValue(false);
            builder.Property(a => a.ShippingStatus).HasDefaultValue(AuctionShippingStatus.NotStarted);
            builder.HasOne(p => p.Payment).WithOne(u => u.Auction).HasForeignKey<Auction>(p => p.PaymentID);
            //buyer won Auctoins
            builder.HasOne(a => a.Buyer).WithMany(u => u.Auctions).HasForeignKey(a => a.BuyerID).IsRequired(false);

            builder.HasOne(i => i.Item).WithOne(a => a.Auction).HasForeignKey<Auction>(i => i.ItemID);

        }
    }
}