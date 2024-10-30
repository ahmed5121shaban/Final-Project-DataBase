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
    public class Admin
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }

    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(i => i.ID);
            builder.HasOne(x => x.User).WithOne(s => s.Admin).HasForeignKey<Admin>(s => s.UserID);
        }
    }
}