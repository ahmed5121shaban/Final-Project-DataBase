using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Final.Enums;


namespace Final
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public DateTime BarthDate { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? Age { get; set; } = 0;
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public Gender? Gender { get; set; }
        public int? NationalId { get; set; } = 0;

        public string? TimeZone { get; set; }
        public string? Description { get; set; }
        public string? Currency {  get; set; }
        public string? Image { get; set; }
        public string? PaymantEmail { get; set; }
        public virtual Seller? Seller { get; set; }
        public virtual Buyer? Buyer { get; set; }
        public virtual Admin? Admin { get; set; }
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
        public virtual ICollection<Notification> Notifacations { get; set; }
        public virtual ICollection<Admin> Admins { get; set; }

    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Name).HasMaxLength(40).IsRequired();
            builder.Property(u => u.City).HasMaxLength(100).IsRequired(false);
            builder.Property(u => u.Country).HasMaxLength(100).IsRequired(false);
            builder.Property(u => u.Age).IsRequired(false);
            builder.Property(u => u.Street).HasMaxLength(200).IsRequired(false);
            builder.Property(u => u.PostalCode).HasMaxLength(20).IsRequired(false);
            builder.Property(u => u.Gender).IsRequired(false);
            builder.Property(u => u.NationalId).IsRequired(false);
            builder.Property(u => u.PaymantEmail).IsRequired(false);
            builder.Property(u => u.TimeZone).HasMaxLength(50).IsRequired(false);
            builder.Property(u => u.Description).HasMaxLength(500).IsRequired(false);

        }
    }
}
