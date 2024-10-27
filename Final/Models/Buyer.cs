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
    public class Buyer
    {
        public int Rate { get; set; }
        public virtual ICollection<FavAuctions> FavAuctions { get; set; }
        public virtual ICollection<FavCategories> FavCategories { get; set; }
        public virtual ICollection<Complain> ComplainesFromSeller { get; set; }

        public string UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Auction> Auctions { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }

    }
    public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.HasKey(x => x.UserID);
            
            builder.HasOne(x => x.User).WithOne(s => s.Buyer).HasForeignKey<Buyer>(s => s.UserID);

            builder.Property(u => u.Rate).HasMaxLength(5).HasDefaultValue(0);
           
        }
    }
}
