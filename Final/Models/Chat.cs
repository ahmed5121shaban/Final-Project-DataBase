using Final.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class Chat
    {
        public int ID { get; set; }
        public int SellerID { get; set; }
        public int BuyerID { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }


        public virtual User Seller { get; set; }
        public virtual User Buyer { get; set; }
        public virtual ICollection<Message> ChatMessages { get; set; }

    }

    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasOne(c=>c.Seller)
                   .WithMany(s=>s.SellerChats)
                   .HasForeignKey(c=>c.SellerID);

            builder.HasOne(c => c.Buyer)
                   .WithMany(s => s.BuyerChats)
                   .HasForeignKey(c => c.BuyerID);

            builder.Property(c => c.StartDate)
                   .IsRequired();

            builder.Property(c => c.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        }
    }
}
