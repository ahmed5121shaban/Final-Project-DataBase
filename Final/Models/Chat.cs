
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace FinalApi
{
    public class Chat
    {
        public int ID { get; set; }
        public string SellerID { get; set; }
        public string BuyerID { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }


        public virtual Seller Seller { get; set; }
        public virtual Buyer Buyer { get; set; }
        public virtual ICollection<Message> ChatMessages { get; set; }

    }

    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            //Seller Chats with Some Buyer
            builder.HasOne(c=>c.Seller)
                   .WithMany(s=>s.Chats)
                   .HasForeignKey(c=>c.SellerID);

            //Buyer Chats with Some Seller
            builder.HasOne(c => c.Buyer)
                   .WithMany(s => s.Chats)
                   .HasForeignKey(c => c.BuyerID);

            builder.Property(c => c.StartDate)
                   .IsRequired();

            builder.Property(c => c.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        }
    }
}
