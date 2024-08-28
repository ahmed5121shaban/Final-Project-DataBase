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
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public DateTime StartDate { get; set; }

        public virtual User Buyer { get; set; }
        public virtual User Seller { get; set; }
        public virtual ICollection<Message> ChatMessages { get; set; }

    }

    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasOne(c=>c.Seller).WithMany(s=>s.Chats).HasForeignKey(c=>c.SellerId);
            builder.HasOne(c => c.Buyer).WithMany(b => b.Chats).HasForeignKey(c => c.BuyerId);
           
        }
    }
}
