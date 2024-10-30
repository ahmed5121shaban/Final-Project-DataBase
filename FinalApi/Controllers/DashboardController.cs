using Managers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AuctionManager auctionManager;
        private readonly ItemManager itemManager;
        private readonly BidManager bidManager;
        private readonly CategoryManager categoryManager;
        private readonly SellerManager sellerManager;

        public DashboardController(AuctionManager _auctionManager,ItemManager _itemManager,BidManager _bidManager,
            CategoryManager _categoryManager,SellerManager _sellerManager)
        {
            auctionManager = _auctionManager;
            itemManager = _itemManager;
            bidManager = _bidManager;
            categoryManager = _categoryManager;
            sellerManager = _sellerManager;
        }

        [HttpGet("ended-completed-auctions")]
        public async Task<IActionResult> GetEndedAndCopletedAuctionsCount()
        {
            int completedAuction = auctionManager.GetAll().Where(a=>a.Completed == true).Count();
            int endedAuction = auctionManager.GetAll().Where(a => a.Ended == true).Count();
            return Ok(new {completedAuction, endedAuction});
        }

        [HttpGet("auctions-amounts")]
        public async Task<IActionResult> GetAllAuctiosAmounts()
        {
            var allBids = bidManager.GetAll().Where(b => b.Auction.Completed == true).Sum(i => i.Amount);
            var allItems = itemManager.GetAll().Where(b => b.Auction.Completed == true).Sum(i => i.StartPrice);
            return Ok(new { allAmount = allBids + allItems });
        }

       [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategoriesItems()
        {
            var categories = categoryManager.GetAll().Select(c => new { name=c.Name, value=c.Items.Count }).ToList();
            return Ok(categories);
        }

        [HttpGet("top-five-sellers")]
        public async Task<IActionResult> GetTopFiveSellers()
        {
            var sellers = sellerManager.GetAll().Where(s => s.Items.Select(i => i.Auction.ItemID)
            .FirstOrDefault() != null).Select(u => new { u.User.Name, u.Items.Count }).OrderByDescending(s => s.Count)
            .Take(5).ToList();

            return Ok(sellers);
        }

        [HttpGet("auctions-bids-amounts")]
        public async Task<IActionResult> AuctionsBidsAmount()
        {
            var auctionsBidsAmount = auctionManager.GetAll().Where(a => a.StartDate < DateTime.Now && a.EndDate > DateTime.Now)
            .Select(a => new { name = a.Item.Name, value = a.Item.StartPrice + a.Bids.Select(b => b.Amount).Sum() }).ToList();
            return Ok(auctionsBidsAmount);
        }

        [HttpGet("last-ten-auction")]
        public async Task<IActionResult> GetLastTenAuction()
        {
            var lastTenAuctions = auctionManager.GetAll()
                .Select(a => new
                {
                    auctionName = a.Item.Name,
                    sellerName = a.Item.Seller.User.Name,
                    buyerName = a.Buyer.User.Name ?? "no buyer for now",
                    images = a.Item.Images,
                    a.Completed,
                    a.Ended,
                    a.StartDate,
                    a.EndDate,
                    a.ID
                }).OrderByDescending(a => a.ID).Take(10).ToList();
            return Ok(lastTenAuctions);
        }
    }
}
