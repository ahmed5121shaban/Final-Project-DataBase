using Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ModelView;

namespace FinalApi
{
    public class BidsHub:Hub
    {
        private readonly BidManager bidManager;
        private readonly UserManager<User> userManager;

        public BidsHub(BidManager _bidManager,UserManager<User> _userManager)
        {
            bidManager = _bidManager;
            userManager = _userManager;
        }


        public async Task AllBids(int auctionId)
        {
            var bids = bidManager.GetAll().Where(b => b.AuctionID == auctionId);
            var user = bids.FirstOrDefault()?.Buyer;
            List<BidViewModel> bidViewModels = new List<BidViewModel>();
            foreach (var bid in bids)
                bidViewModels.Add(bid.ToBidViewModel());

            await Clients.Group(auctionId.ToString()).SendAsync("AllBids", bidViewModels);
        }

       

        public async Task JoinGroup(int auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, auctionId.ToString());
        }

        public async Task LeaveGroup(string auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId.ToString());
        }
    }
}
