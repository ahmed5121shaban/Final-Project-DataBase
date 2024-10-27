using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FinalApi
{
    public class PhoneNumber
    {
        public int ID { get; set; }
        public string Phone { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }

    public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
    {
        public void Configure(EntityTypeBuilder<PhoneNumber> builder)
        {
            builder.HasOne(p=>p.User).WithMany(u=>u.PhoneNumbers).HasForeignKey(p=>p.UserID).OnDelete(DeleteBehavior.Cascade);
            builder.Property(p => p.UserID).IsRequired();
            builder.Property(p => p.Phone).IsRequired().HasMaxLength(15);
            builder.HasKey(p => p.ID);

        }
    }
}