using Final;
using finalproject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final.Models;

namespace zaeid.models
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
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int? EventID { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Admin> Admins { get; set; }


    }

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasOne(i=>i.User).WithMany(u=>u.Items).HasForeignKey(i=>i.UserID);
            builder.HasOne(i => i.Event).WithMany(e => e.Items).HasForeignKey(i => i.EventID);
        }
    }
}
