
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
    public class Image
    {
        public int ID { get; set; }
        public string Src {  get; set; }

        public int ItemID { get; set; }
        public virtual Item Item { get; set; }


    }

    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.ID);
            builder.Property(i => i.Src).HasMaxLength(300).IsRequired();
            builder.Property(i => i.ItemID).IsRequired();
            builder.HasOne(i=>i.Item).WithMany(u=>u.Images).HasForeignKey(i=>i.ItemID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
