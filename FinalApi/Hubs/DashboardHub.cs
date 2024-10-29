using Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace FinalApi
{
    public class DashboardHub:Hub
    {
        private readonly AuctionManager auctionManager;
        private readonly UserManager<User> userManager;
        private readonly SellerManager sellerManager;
        private readonly CategoryManager categoryManager;

        public DashboardHub(AuctionManager _auctionManager,UserManager<User> _userManager,SellerManager _sellerManager,
            CategoryManager _categoryManager)
        {
            auctionManager = _auctionManager;
            userManager = _userManager;
            sellerManager = _sellerManager;
            categoryManager = _categoryManager;
        }
        public async Task Dashboard()
        {
            try { 
            var Auctions = auctionManager.GetAll().Count();
            await Clients.All.SendAsync("AuctionsCount", Auctions);

                var sellers = sellerManager.GetAll().Where(s => s.Items.Select(i => i.Auction.ItemID)
               .FirstOrDefault() != null).Select(u => new { u.User.Name, u.Items.Count }).OrderByDescending(s => s.Count)
               .Take(5).ToList();
                await Clients.All.SendAsync("topFiveSeller", sellers);

                var categories = categoryManager.GetAll().Select(c => new { name = c.Name, value = c.Items.Count }).ToList();
                await Clients.All.SendAsync("category", categories);

                

                var auctionsBidsAmount = auctionManager.GetAll().Where(a => a.StartDate > DateTime.Now && a.EndDate < DateTime.Now)
                    .Select(a => new { name = a.Item.Name, value = a.Item.StartPrice + a.Bids.Select(b => b.Amount).Sum() }).ToList();
                await Clients.All.SendAsync("auctionsBidsAmount", auctionsBidsAmount);
            }
            catch (Exception ex) { };
        }

       
       
    }
}
