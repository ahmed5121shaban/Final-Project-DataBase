using Managers;
using Microsoft.AspNetCore.SignalR;

namespace FinalApi
{
    public class TableDashboardHub:Hub
    {
        private readonly AuctionManager auctionManager;

        public TableDashboardHub(AuctionManager _auctionManager)
        {
            auctionManager = _auctionManager;
        }
        //the connection closed when run this request in this method to display the table 
        public async Task TableData()
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
            await Clients.All.SendAsync("lastTenAuctions", lastTenAuctions);
        }
    }
}
