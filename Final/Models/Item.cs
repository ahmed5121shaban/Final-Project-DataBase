using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FinalApi.Enums;
using FinalApi;

namespace FinalApi
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
        public string Description { get; set; }
        public DateTime AddTime { get; set; }
        public ItemStatus Status { get; set; }
        public string? PublishFeedback { get; set; }
        public DateTime? PublishDate { get; set; }
        public decimal EndPrice { get; set; }
        public decimal StartPrice { get; set; }
        public string ContractFile {  get; set; }
        public int? AuctionID { get; set; }
        public virtual Auction Auction {  get; set; }
        public string SellerID { get; set; }
        public virtual Seller Seller { get; set; }
        public int? EventID { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public bool Deleted { get; set; }
        

    }

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(i => i.ID);
            builder.Property(i => i.Name).IsRequired();
            builder.Property(i => i.StartPrice).IsRequired();
            builder.Property(i => i.EndPrice).IsRequired();
            builder.Property(i => i.SellerID).IsRequired();
            builder.Property(i => i.CategoryID).IsRequired();
            builder.Property(i => i.PublishDate).IsRequired(false);
            builder.Property(i => i.Status).IsRequired().HasDefaultValue(ItemStatus.pending);
            //seller has many items
            builder.HasOne(i=>i.Seller).WithMany(u=>u.Items).HasForeignKey(i=>i.SellerID);
            builder.HasOne(i => i.Event).WithMany(e => e.Items).HasForeignKey(i => i.EventID);
            builder.HasOne(i => i.Category).WithMany(c => c.Items).HasForeignKey(i => i.CategoryID);
            builder.HasOne(i => i.Auction).WithOne(a=>a.Item).HasForeignKey<Item>(i=>i.AuctionID);
            builder.Property(i => i.Deleted).HasDefaultValue(false);
        }
    }
}
