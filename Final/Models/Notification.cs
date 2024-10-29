
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
    public class Notification
    {
        //adding string type in title remove enum
        public int Id { get; set; }
        public Enums.NotificationType Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsReaded { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }

    }

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Title).IsRequired().HasConversion<string>();
            builder.Property(n => n.Date).IsRequired();
            builder.Property(n => n.Description).IsRequired().HasMaxLength(500);
            builder.Property(n => n.IsReaded).IsRequired();
            builder.Property(n => n.UserId).IsRequired();
            builder.HasOne(n=>n.User).WithMany(u=>u.Notifacations).HasForeignKey(n=>n.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
