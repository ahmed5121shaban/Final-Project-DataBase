using Final;
using System.Linq;
using Models;
using Models.Models;
using ModelView;
using ModelView.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Managers
{
    public class ProfileManager:MainManager<User>
    {
        private readonly FinalDbContext _context;

        public ProfileManager(FinalDbContext context) : base(context)
        {
            _context = context;
        }

        public SellerProfileViewModel GetSellerProfile(int userId)
        {
            var seller = _context.Seller
                .Include(s => s.User)
                .Include(s => s.Items)
                .ThenInclude(i => i.Reviews)  // إضافة مراجعات العناصر
                .FirstOrDefault(s => s.UserID == userId.ToString());

            if (seller == null) return null;

            return new SellerProfileViewModel
            {
                UserId = seller.UserID,
                Name = seller.User.Name,
                Email = seller.User.Email,
                ImageUrl = seller.User.Image,
                CompletedAuctions = _context.Auctions.Count(a => a.Item.SellerID == seller.UserID && a.Completed),
                UnfinishedAuctions = _context.Auctions.Count(a => a.Item.SellerID == seller.UserID && !a.Completed),
                SellerRating = seller.Items.SelectMany(i => i.Reviews).Any()
                               ? seller.Items.SelectMany(i => i.Reviews).Average(r => r.Rating)
                               : 0,
                ProfileCompletion = CalculateProfileCompletion(seller.User)
            };
        }

        public BuyerProfileViewModel GetBuyerProfile(int userId)
        {
            var buyer = _context.Buyer
                .Include(b => b.User)
                .Include(b => b.Bids)
                .Include(b => b.Auctions)
                .FirstOrDefault(b => b.UserID == userId.ToString());

            if (buyer == null) return null;

            return new BuyerProfileViewModel
            {
                UserId = buyer.UserID,
                Name = buyer.User.Name,
                Email = buyer.User.Email,
                ImageUrl = buyer.User.Image,
                WonAuctions = _context.Auctions.Count(a => a.Buyer.UserID == buyer.UserID),
                LostAuctions = _context.Auctions.Count(a => a.Buyer.UserID != buyer.UserID && a.Bids.Any(b => b.Buyer.UserID == buyer.UserID)),
                ProfileCompletion = CalculateProfileCompletion(buyer.User)
            };
        }

        private decimal CalculateProfileCompletion(User user)
        {
            // منطق حساب إكمال البروفايل
            return 80; // نسبة افتراضية
        }
    }
}
