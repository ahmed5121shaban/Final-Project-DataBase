using FinalApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinalApi
{
    public class FinalDbContext: IdentityDbContext<User>
    {
        public FinalDbContext(DbContextOptions<FinalDbContext> _contextOptions):base(_contextOptions)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfiguration(new BidConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ChatConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new PhoneNumberConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
            modelBuilder.ApplyConfiguration(new AuctionConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new SellerConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerConfiguration());
            modelBuilder.ApplyConfiguration(new FavCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new FavAuctionConfiguration());
            modelBuilder.ApplyConfiguration(new ComplainConfiguration());

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" },
                new IdentityRole { Name = "Seller", NormalizedName = "SELLER" },
                new IdentityRole { Name = "Buyer", NormalizedName = "BUYER" }
            );
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    ID =1,
                    Name = "Cars",
                    Description = "Description scripe scripe scripe scripe scripe scripe scripe",
                    Image = "https://picsum.photos/seed/picsum/214/300"
                },
                new Category
                {
                    ID = 2,
                    Name = "Food",
                    Description = "Description scripe scripe scripe scripe scripe scripe scripe",
                    Image = "https://picsum.photos/seed/picsum/213/300"
                },
                 new Category
                 {
                     ID = 3,
                     Name = "Electronic",
                     Description = "Description scripe scripe scripe scripe scripe scripe scripe",
                     Image = "https://picsum.photos/seed/picsum/212/300"
                 },
                  new Category
                  {
                      ID = 4,
                      Name = "Cloths",
                      Description = "Description scripe scripe scripe scripe scripe scripe scripe",
                      Image = "https://picsum.photos/seed/picsum/211/300"
                  },
                   new Category
                   {
                       ID = 5,
                       Name = "Toy",
                       Description = "Description scripe scripe scripe scripe scripe scripe scripe",
                       Image = "https://picsum.photos/seed/picsum/210/300"
                   },
                   new Category
                   {
                       ID = 6,
                       Name = "Others",
                       Description = "Description scripe scripe scripe scripe scripe scripe scripe",
                       Image = "https://picsum.photos/seed/picsum/201/300"
                   }
           );
           
        }


        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Seller> Seller { get; set; }
        public virtual DbSet<Buyer> Buyer { get; set; }
        public virtual DbSet<FavAuctions> FavAuctions { get; set; }
        public virtual DbSet<FavCategories> FavCategories { get; set; }
        public virtual DbSet<Complain> Complains { get; set; }
        public virtual DbSet<Review> Review { get; set; }

    }
}
