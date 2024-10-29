using FinalApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinalApi
{
    public class Review
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public byte Range {  get; set; }  
        public DateTime Date { get; set; }
        public string SellerID { get; set; }
        public virtual Seller Seller { get; set; }
        public string BuyerID { get; set; }
        public virtual Buyer Buyer { get; set; }
        public int AuctionID { get; set; }
        public virtual Auction Auction { get; set; }
    }

    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(x => x.ID);
            //seller take review
            builder.HasOne(x => x.Seller).WithMany(x => x.Reviews).HasForeignKey(r=>r.SellerID);
            // buyer add review
            builder.HasOne(x => x.Buyer).WithMany(x => x.Reviews).HasForeignKey(r=>r.BuyerID);

            builder.HasOne(x => x.Auction)
               .WithOne(a => a.Review)
               .HasForeignKey<Review>(x => x.AuctionID);
        }
    }
}
