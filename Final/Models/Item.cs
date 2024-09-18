using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
        public string Description { get; set; }
        public DateTime AddTime { get; set; }
        public bool IsReviewed { get; set; }
        public DateTime PublishDate { get; set; }
        public decimal EndPrice { get; set; }
        public decimal StartPrice { get; set; }
        public string ContractFile {  get; set; }
        public int AuctionID { get; set; }
        public virtual Auction Auction {  get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
        public int? EventID { get; set; }
        public virtual Event Event { get; set; }
        public int ReviewID {  get; set; }
        public virtual Review Review { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<Buyer> Buyers { get; set; }




    }

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(i => i.ID);
            builder.Property(i => i.Name).IsRequired();
            builder.Property(i => i.StartPrice).IsRequired();
            builder.Property(i => i.EndPrice).IsRequired();
            builder.Property(i => i.UserID).IsRequired();
            builder.Property(i => i.CategoryID).IsRequired();
            builder.Property(i => i.IsReviewed).IsRequired().HasDefaultValue(false);
            builder.HasOne(i=>i.User).WithMany(u=>u.Items).HasForeignKey(i=>i.UserID);
            builder.HasOne(i => i.Event).WithMany(e => e.Items).HasForeignKey(i => i.EventID);
            builder.HasOne(i => i.Review).WithOne(i => i.Item).HasForeignKey<Item>(i => i.ReviewID);
            builder.HasOne(i => i.Category).WithMany(c => c.Items).HasForeignKey(i => i.CategoryID);
            
        }
    }
}
