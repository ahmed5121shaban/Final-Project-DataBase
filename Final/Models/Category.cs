using FinalApi;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;

namespace FinalApi
{
    public class Category
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string ?Icon { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<FavCategories> FavCategories { get; set; }

    }

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired() 
                   .HasMaxLength(50); 

            builder.Property(c => c.Image)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(c => c.Description)
                   .IsRequired()
                   .HasMaxLength(1000);
        }
    }
}
