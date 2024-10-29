using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;
using FinalApi;

namespace FinalApi
{
    public class Seller
    {
        public string UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Complain> MyComplainesonBuyer { get; set; }

        public decimal? WithdrawnAmount { get; set; }


    }
    public class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.HasKey(x => x.UserID);
            builder.HasOne(x => x.User).WithOne(s => s.Seller).HasForeignKey<Seller>(s => s.UserID);
        }
    }
}
