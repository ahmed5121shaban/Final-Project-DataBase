using Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace FinalApi
{
    public class DashboardHub:Hub
    {
        private readonly AuctionManager auctionManager;
        private readonly UserManager<User> userManager;

        public DashboardHub(AuctionManager _auctionManager,UserManager<User> _userManager)
        {
            auctionManager = _auctionManager;
            userManager = _userManager;
        }
        public async Task Dashboard()
        {
            var Auctions = auctionManager.GetAll().ToList();
            await Clients.All.SendAsync("AllAuction", Auctions);
            var allUsers = auctionManager.GetAll().Count();
            await Clients.All.SendAsync("AllUsersCount", allUsers);
            await Clients.All.SendAsync("SalesOfYear", null);
        }
    }
}
