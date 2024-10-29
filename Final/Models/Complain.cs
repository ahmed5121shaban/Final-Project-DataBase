
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace Models.Models
{
    public class Complain
    {
        public int ID { get; set; }
        public string Reason { get; set; }
        public string SellerID { get; set; }
        public virtual Seller Seller { get; set; }
        public string BuyerID { get; set; }
        public virtual Buyer Buyer { get; set; }

    }

    public class ComplainConfiguration : IEntityTypeConfiguration<Complain>
    {
        public void Configure(EntityTypeBuilder<Complain> builder)
        {
            builder.HasKey(x => x.ID);
            builder.HasOne(x => x.Seller).WithMany(x => x.MyComplainesonBuyer).HasForeignKey(r => r.SellerID);
            builder.HasOne(x => x.Buyer).WithMany(x => x.ComplainesFromSeller).HasForeignKey(r => r.BuyerID);
        }
    }
}
