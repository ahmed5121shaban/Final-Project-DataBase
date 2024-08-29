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
        public int UserID { get; set; }
        public DateTime StartDate { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Message> ChatMessages { get; set; }

    }

    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasOne(c=>c.User).WithMany(s=>s.Chats).HasForeignKey(c=>c.UserID);
           
        }
    }
}
