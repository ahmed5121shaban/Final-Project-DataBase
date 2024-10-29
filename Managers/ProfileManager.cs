using System.Linq;
using Models;
using Models.Models;
using ModelView;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelView.Account;
using Microsoft.IdentityModel.Tokens;
using FinalApi;
using static FinalApi.Enums;

namespace Managers
{
    public class ProfileManager : MainManager<User>
    {
        private readonly FinalDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProfileManager(FinalDbContext context, UserManager<User> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<SellerProfileViewModel> GetSellerProfile(string userId)
        {
            var seller = await _context.Seller
                .Where(s => s.UserID == userId)
                .Select(s => new
                {
                    s.User,
                    Items = s.Items.Select(i => i.toItemViewModel()).ToList()
                }).FirstOrDefaultAsync();

            if (seller == null) return null;

            var completedAuctions = CalculateCompletedAuctions(seller.User.Id); // تعديل هنا
            var unfinishedAuctions = CalculateUnfinishedAuctions(seller.User.Id);
            var sellerRating = CalculateSellerRating(seller.User.Id);
            var profileCompletion = CalculateProfileCompletion(seller.User);

            return new SellerProfileViewModel
            {
                UserId = seller.User.Id, // تعديل هنا
                Name = seller.User.Name,
                Email = seller.User.Email,
                ImageUrl = seller.User.Image,
                CompletedAuctions = (int)completedAuctions,
                UnfinishedAuctions = (int)unfinishedAuctions,
                SellerRating = (int)sellerRating,
                ProfileCompletion = (int)profileCompletion,
                Items = seller.Items
            };
        }

        public async Task<BuyerProfileViewModel> GetBuyerProfile(string userId)
        {
            var buyer = await _context.Buyer
                .Where(b => b.UserID == userId)
                .Select(b => new
                {
                    b.User,
                    Auctions = b.Auctions.Select(a => new AuctiondetailsViewModel
                    {
                        ID = a.ID,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        ItemID = a.ItemID,
                        Item = new AuctionItemViewModel
                        {
                            ID = a.Item.ID,
                            StartPrice = a.Item.StartPrice,
                            status = a.Item.Status,
                            Name = a.Item.Name,
                            AddTime = a.Item.AddTime,
                            Images = a.Item.Images.Select(i => i.Src).ToList(),
                            Description = a.Item.Description,
                            CategoryID = a.Item.CategoryID,
                            Category = a.Item.Category.Name,
                            SellerId = a.Item.SellerID,
                            SellerName = a.Item.Seller.User.Name,
                        }
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (buyer == null) return null;

            var wonAuctions = CalculateWonAuctions(buyer.User.Id); // تعديل هنا
            var lostAuctions = CalculateLostAuctions(buyer.User.Id);
            var profileCompletion = CalculateProfileCompletion(buyer.User);

            return new BuyerProfileViewModel
            {
                UserId = buyer.User.Id, // تعديل هنا
                Name = buyer.User.Name,
                Email = buyer.User.Email,
                ImageUrl = buyer.User.Image,
                WonAuctions = (int)wonAuctions,
                LostAuctions = (int)lostAuctions,
                ProfileCompletion = (int)profileCompletion,
                Auctions = buyer.Auctions
            };
        }

        private decimal CalculateSellerRating(string userId)
        {
            var totalReviews = _context.Review.Count(r => r.SellerID == userId);
            return totalReviews > 0 ?
                   Math.Round((decimal)_context.Review.Where(r => r.SellerID == userId).Average(r => r.Range) * 100 / 5) : 0; // Assuming the rating is out of 5
        }

        private decimal CalculateCompletedAuctions(string userId)
        {
            var totalAuctions = _context.Auctions.Count(a => a.Item.SellerID == userId);
            return totalAuctions > 0 ?
                   Math.Round((decimal)_context.Auctions.Count(a => a.Item.SellerID == userId && a.Completed) * 100 / totalAuctions) : 0;
        }

        private decimal CalculateUnfinishedAuctions(string userId)
        {
            var totalAuctions = _context.Auctions.Count(a => a.Item.SellerID == userId);
            return totalAuctions > 0 ?
                   Math.Round((decimal)_context.Auctions.Count(a => a.Item.SellerID == userId && !a.Completed) * 100 / totalAuctions) : 0;
        }

        private decimal CalculateWonAuctions(string userId)
        {
            var totalAuctions = _context.Auctions.Count(a => a.BuyerID == userId);
            return totalAuctions > 0 ?
                   Math.Round((decimal)_context.Auctions.Count(a => a.BuyerID == userId) * 100 / totalAuctions) : 0;
        }

        private decimal CalculateLostAuctions(string userId)
        {
            var totalAuctions = _context.Auctions.Count(a => a.BuyerID != userId && a.Bids.Any(bid => bid.BuyerID == userId));
            return totalAuctions > 0 ?
                   Math.Round((decimal)_context.Auctions.Count(a => a.BuyerID != userId && a.Bids.Any(bid => bid.BuyerID == userId)) * 100 / totalAuctions) : 0;
        }

        private static decimal CalculateProfileCompletion(User user)
        {
            int totalFields = 13; // تحديث العدد بناءً على عدد الحقول
            int filledFields = 0;

            if (!string.IsNullOrEmpty(user.Name)) filledFields++;
            if (!string.IsNullOrEmpty(user.Image)) filledFields++;
            if (!string.IsNullOrEmpty(user.Email)) filledFields++;
            if (!string.IsNullOrEmpty(user.City)) filledFields++;
            if (!string.IsNullOrEmpty(user.Country)) filledFields++;
            if (!string.IsNullOrEmpty(user.Street)) filledFields++;
            if (!string.IsNullOrEmpty(user.PostalCode)) filledFields++;
            if (!string.IsNullOrEmpty(user.TimeZone)) filledFields++;
            if (!string.IsNullOrEmpty(user.Currency)) filledFields++;
            if (user.PhoneNumbers != null && user.PhoneNumbers.Count > 0) filledFields++;
            if (!string.IsNullOrEmpty(user.Description)) filledFields++; // إذا كانت الوصف محددة
            if (user.Gender == Gender.male || user.Gender == Gender.female) filledFields++; // إذا كان الجنس محدد
            if (user.Age > 0) filledFields++; // إذا كان العمر محدد

            return Math.Round((decimal)filledFields / totalFields * 100);
        }


    }

}
