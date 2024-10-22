using Managers;
using Microsoft.AspNetCore.SignalR;
using ModelView;

namespace FinalApi
{
    public class BidsHub:Hub
    {
        private readonly BidManager bidManager;

        public BidsHub(BidManager _bidManager)
        {
            bidManager = _bidManager;
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
