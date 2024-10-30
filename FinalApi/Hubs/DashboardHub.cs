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

            }
            catch (Exception ex) { };
        }

       
       
    }
}
