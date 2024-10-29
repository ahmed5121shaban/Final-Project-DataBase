using FinalApi;
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
using static FinalApi.Enums;


namespace FinalApi
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
        public string? NationalId { get; set; }
        public string? NationalIdFrontImage { get; set; } 
        public string? NationalIdBackImage { get; set; }
        public bool IsBlocked { get; set; }
        public int Reports { get; set; }
        public string? TimeZone { get; set; }
        public string? Description { get; set; }
        public string? Currency { get; set; } = "EGP";
        public string? Image { get; set; }
        //user have tow payment method
        public string? PaypalEmail { get; set; }
        public string? StripeEmail { get; set; }
        public virtual Seller? Seller { get; set; }
        public virtual Buyer? Buyer { get; set; }
        public virtual Admin? Admin { get; set; }
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
        public virtual ICollection<Notification> Notifacations { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

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
            builder.Property(u => u.Gender).HasConversion<string>().IsRequired(false);
            builder.Property(u => u.NationalId).IsRequired(false);
            builder.Property(u => u.TimeZone).HasMaxLength(50).IsRequired(false);
            builder.Property(u => u.Description).HasMaxLength(500).IsRequired(false);
            builder.Property(u => u.IsBlocked).HasDefaultValue(false);
            builder.Property(u => u.Reports).HasDefaultValue(0);
        }
    }
}
