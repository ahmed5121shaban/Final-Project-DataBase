using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using ModelView;
namespace FinalApi
{
    public class HangfireManager
    {
        private readonly AuctionManager auctionManager;
        private readonly PaymentManager paymentManager;
        private readonly BidManager bidManager;
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly NotificationManager notificationManager;
        private readonly ChatManager chatManager;

        public HangfireManager(AuctionManager _auctionManager,PaymentManager _paymentManager,
            BidManager _bidManager,IHubContext<NotificationsHub> _hubContext,NotificationManager _notificationManager,
            ChatManager _chatManager)
        {
            auctionManager = _auctionManager;
            paymentManager = _paymentManager;
            bidManager = _bidManager;
            hubContext = _hubContext;
            notificationManager = _notificationManager;
            chatManager = _chatManager;
        }
        public async Task EndAuctionAtTime(int _auctionID)
        {
            //ended column true && without buyer => ended only || with buyer => (completed|not) | (expected salary)
            var auction =await auctionManager.GetOne(_auctionID);
            if(auction==null) return;

            auction.Ended = true;
            if(await auctionManager.Update(auction)==false) return;

            var bid = auction.Bids.LastOrDefault();
            if (bid == null) return;

            var payment = paymentManager.GetAll().FirstOrDefault(p=>p.AuctionID==_auctionID&&p.BuyerId==bid.BuyerID);
            if (payment==null) return;

            var bids = bidManager.GetAll().Where(b=>b.AuctionID == payment.Id).Select(b=>b.Amount).ToList();
            if( bids.Sum() < auction.Item.EndPrice) return;

            payment.IsDone= true;
            if(await paymentManager.Update(payment)) return;

            await chatManager.Add(new Chat
            {
                IsActive = false,
                BuyerID=bid.BuyerID,
                SellerID = auction.Item.SellerID,
                StartDate = DateTime.Now,
            });
            //refund the payment to all lost buyer

            //when the auction done (the time end with buyer bid more than expected price) i must add the buyer id and payment id
            auction.BuyerID = payment.BuyerId;
            auction.PaymentID = payment.Id; 
            await auctionManager.Update(auction);

        }

        public async Task AuctionEndedNotificationBeforeOneDay(int _auctionID, string groupName)
        {
            await notificationManager.Add(new Notification
             {
               Date = DateTime.Now,
               Description = "This Auction have One Day To End",
               IsReaded = false,
               Title = Enums.NotificationType.auction,
               UserId = groupName,
            });
           
            Notification lastNotification = notificationManager.GetAll().Where(n=>n.UserId==groupName).OrderBy(n => n.Id).LastOrDefault();
            if (lastNotification == null)
                return ;

            await hubContext.Clients.Groups(groupName).SendAsync("notification", lastNotification.ToViewModel());
        }
    }
}
