using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Final.Models
{
    public class PhoneNumber
    {
        public int ID { get; set; }
        public string Phone { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }

    public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
    {
        public void Configure(EntityTypeBuilder<PhoneNumber> builder)
        {

        }
    }
}