using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Final
{
    public class Review
    {
        public int ID { get; set; }
        public string Descrip { get; set; }
        public byte Range {  get; set; }  
        public int UserID { get; set; }
        public User User { get; set; }
        public int ItemID { get; set; }
     
        public Item Item { get; set; }
    }

    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(x => x.ID);
            builder.HasOne(x => x.User).WithMany(x => x.Reviews).HasForeignKey(r=>r.UserID);
            builder.HasOne(x => x.Item).WithOne(x => x.Review).HasForeignKey<Review>(r=>r.ItemID);
        }
    }
}
