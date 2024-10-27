using FinalApi;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class FavAuctions
    {
        public int ID{ get; set; }
        public int AuctionID { get; set; }
        public virtual Auction Auction { get; set; }
        public string BuyerID { get; set; }
        public virtual Buyer Buyer { get; set; }

    }
    public class FavAuctionConfiguration : IEntityTypeConfiguration<FavAuctions>
    {
        public void Configure(EntityTypeBuilder<FavAuctions> builder)
        {
            builder.HasKey(a => a.ID);
            builder.Property(a => a.ID).ValueGeneratedOnAdd();
            builder.HasOne(a => a.Buyer).WithMany(u => u.FavAuctions).HasForeignKey(a => a.BuyerID);

            builder.HasOne(i => i.Auction).WithMany(a => a.FavAuctions).HasForeignKey(i => i.AuctionID);

        }
    }
}
