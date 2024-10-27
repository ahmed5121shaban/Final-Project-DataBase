
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FinalApi
{
    public class Payment
    {
        public int Id { get; set; }
        public Enums.PaymentMetod Method {  get; set; }
        public bool IsDone { get; set; }
        public string BuyerId { get; set; }
        [JsonIgnore]
        public virtual Buyer Buyer { get; set; }
        public int? AuctionID {  get; set; }
        [JsonIgnore]
        public virtual Auction Auction { get; set; }

    }

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Method).IsRequired().HasConversion<string>();
            builder.Property(p => p.IsDone).IsRequired().HasDefaultValue(false);
            builder.Property(p => p.BuyerId).IsRequired();
            builder.Property(p => p.AuctionID).IsRequired(false);
            builder.HasOne(p=>p.Buyer).WithMany(u=>u.Payments).HasForeignKey(p=>p.BuyerId);
            builder.HasOne(p => p.Auction).WithOne(u => u.Payment).HasForeignKey<Payment>(p=>p.AuctionID);
        }
    }
}
