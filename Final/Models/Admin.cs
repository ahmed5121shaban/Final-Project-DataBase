using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class Admin
    {
        public string UserID { get; set; }
        public User User { get; set; }


        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(i => i.ID);
            builder.Property(i => i.Name).IsRequired();
            builder.Property(i => i.Email).IsRequired();
            builder.Property(i => i.Password).IsRequired();


        }
    }
}