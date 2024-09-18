using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class Buyer
    {
        public string ID { get; set; }
        public int Rate { get; set; }
        public virtual ICollection<Item> SavedItems { get; set; }
        public virtual ICollection<Category> SavedCategories { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }
    public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.HasKey(x => x.ID);
            builder.HasOne(x => x.User).WithOne(s => s.Buyer).HasForeignKey<Buyer>(s => s.UserID);
            builder.Property(u => u.Rate).HasMaxLength(5).HasDefaultValue(0);
        }
    }
}
