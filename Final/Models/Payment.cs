
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class Payment
    {
        public int Id { get; set; }
        public Enums.PaymentMetod Method {  get; set; }
        public bool IsDone { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int AuctionID {  get; set; }
        public virtual Auction Auction { get; set; }

    }

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Method).IsRequired().HasConversion<string>();
            builder.Property(p => p.IsDone).IsRequired().HasDefaultValue(false);
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.AuctionID).IsRequired();
            builder.HasOne(p=>p.User).WithMany(u=>u.Payments).HasForeignKey(p=>p.UserId);
            builder.HasOne(p => p.Auction).WithOne(u => u.Payment).HasForeignKey<Payment>(p=>p.UserId);
        }
    }
}
