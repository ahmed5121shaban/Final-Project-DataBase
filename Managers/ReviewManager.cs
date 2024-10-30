
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;
using System.Data.Entity;

namespace Managers
{
    public class ReviewManager:MainManager<Review>
    {
        private readonly FinalDbContext dbcontext;
        private DbSet<Review> dbset;
        public ReviewManager(FinalDbContext _context) : base(_context)
        {
            dbcontext = _context;
        }
        public bool HasUserReviewedAuction(string userId, int auctionId)
        {
            return dbcontext.Review.Any(review => review.BuyerID == userId && review.AuctionID == auctionId);
        }
    }
}
