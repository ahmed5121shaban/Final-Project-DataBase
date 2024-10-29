
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace Models.Models
{
    public class FavCategories
    {

            public int ID { get; set; }
            public int CategoryID { get; set; }
            public virtual Category Category { get; set; }

            public string BuyerID { get; set; }
            public virtual Buyer Buyer { get; set; }

    }
        public class FavCategoryConfiguration : IEntityTypeConfiguration<FavCategories>
        {
            public void Configure(EntityTypeBuilder<FavCategories> builder)
            {
                builder.HasKey(a => a.ID);
                builder.Property(a => a.ID).ValueGeneratedOnAdd();
                builder.HasOne(a => a.Buyer).WithMany(u => u.FavCategories).HasForeignKey(a => a.BuyerID);

                builder.HasOne(i => i.Category).WithMany(a => a.FavCategories).HasForeignKey(i => i.CategoryID);

            }
        
        }
}
