﻿using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<NotificationsHub> notificationsHub;
        private readonly NotificationManager notificationManager;
        private readonly ChatManager chatManager;
        private readonly IHubContext<DashboardHub> dashboardHub;

        public HangfireManager(AuctionManager _auctionManager,PaymentManager _paymentManager,
            BidManager _bidManager,IHubContext<NotificationsHub> _notificationsHub, 
            IHubContext<DashboardHub> _dashboardHub,
            NotificationManager _notificationManager,
            ChatManager _chatManager)
        {
            auctionManager = _auctionManager;
            paymentManager = _paymentManager;
            bidManager = _bidManager;
            notificationsHub = _notificationsHub;
            notificationManager = _notificationManager;
            chatManager = _chatManager;
            dashboardHub = _dashboardHub;
        }
        public async Task EndAuctionAtTime(int _auctionID)
        {
            var auction =await auctionManager.GetOne(_auctionID);
            if(auction==null) return;

            auction.Ended = true;
            if(await auctionManager.Update(auction)==false) return;
            await dashboardHub.Clients.All.SendAsync("endedAuction",1);

            //send to seller his auction is ended
            await notificationManager.Add(new Notification
            {
                Date = DateTime.Now,
                Description = $"Your Auction is Ended {auction.Item.Name}" ,
                IsReaded = false,
                Title = Enums.NotificationType.auction,
                UserId = auction.Item.SellerID
            });
            var lastNotification = notificationManager.GetAll().Where(n=>n.UserId == auction.Item.SellerID).OrderBy(n=>n.Id).LastOrDefault();
            await notificationsHub.Clients.Group(auction.Item.SellerID).SendAsync("notification", lastNotification.ToViewModel());

            var bid = auction.Bids.LastOrDefault();
            if (bid == null) return;

            var payment = paymentManager.GetAll().FirstOrDefault(p=>p.AuctionID==_auctionID&&p.BuyerId==bid.BuyerID);
            if (payment==null) return;
            //paymen.AuctionID insted of payment.ID
            var bids = bidManager.GetAll().Where(b=>b.AuctionID == payment.AuctionID).Select(b=>b.Amount);
            if( bids.Sum() < auction.Item.EndPrice) return;

            payment.IsDone= true;
            if(!await paymentManager.Update(payment)) return;
            //send notification to won user
            await notificationManager.Add(new Notification
            {
                Date = DateTime.Now,
                Description = $"your Auction {auction.Item.Name} is Done ,Complete The payment",
                IsReaded = false,
                Title = Enums.NotificationType.auction,
                UserId = payment.BuyerId,
            });
            var lastNotify = notificationManager.GetAll().Where(n => n.UserId == payment.BuyerId).OrderBy(n => n.Id).LastOrDefault();
            await notificationsHub.Clients.Group(payment.BuyerId).SendAsync("notification", lastNotify.ToViewModel());


            var chat = chatManager.GetAll().Where(c => c.BuyerID == bid.BuyerID && c.SellerID == auction.Item.SellerID);
            if (!chat.Any())
                await chatManager.Add(new Chat
                {
                    IsActive = false,
                    BuyerID = bid.BuyerID,
                    SellerID = auction.Item.SellerID,
                    StartDate = DateTime.Now,
                });

            var refundAmount = auction.Item.StartPrice;
            var lostBuyersEmails = paymentManager.GetAll().Where(p=>p.AuctionID == _auctionID&&p.IsDone == false).Count();

            var result = paymentManager.RefundCustomerAmount("gamal-gamal@personal.example.com", refundAmount * lostBuyersEmails);
            //an error here
            //if (result.statusCode == 400) return;

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

            await notificationsHub.Clients.Groups(groupName).SendAsync("notification", lastNotification.ToViewModel());
        }

        public async Task LostAuctionNotifications(int _auctionID)
        {
           var paymentUsersIDs = paymentManager.GetAll().Where(p=>p.AuctionID == _auctionID&&p.IsDone == false)
                .Select(p=>new { p.BuyerId ,p.Auction.Item.Name});
            if (!paymentUsersIDs.Any()) return;
            foreach (var payment in paymentUsersIDs)
            {
                await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = $"You Lose this Auction {payment.Name}",
                    IsReaded = false,
                    Title = Enums.NotificationType.auction,
                    UserId = payment.BuyerId,
                });
                var lastNotification = notificationManager.GetAll().Where(n => n.UserId == payment.BuyerId).OrderBy(n => n.Id)
                    .LastOrDefault();
                if (lastNotification == null)
                    return;

                await notificationsHub.Clients.Groups(payment.BuyerId).SendAsync("notification", lastNotification.ToViewModel());
            }


        }
    }
}
