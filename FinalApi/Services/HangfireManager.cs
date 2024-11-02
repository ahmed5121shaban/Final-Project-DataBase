using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using ModelView;
using Hangfire;
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
        //public async Task EndAuctionAtTime(int _auctionID)
        //{
        //    var auction =await auctionManager.GetOne(_auctionID);
        //    if(auction==null) return;

        //    auction.Ended = true;
        //    if(await auctionManager.Update(auction)==false) return;
        //    await dashboardHub.Clients.All.SendAsync("endedAuction",1);

        //    var bid = auction.Bids.LastOrDefault();
        //    if (bid == null) return;

        //    var payment = paymentManager.GetAll().FirstOrDefault(p=>p.AuctionID==_auctionID&&p.BuyerId==bid.BuyerID);
        //    if (payment==null) return;
        //    //paymen.AuctionID insted of payment.ID
        //    var bids = bidManager.GetAll().Where(b=>b.AuctionID == payment.AuctionID).Select(b=>b.Amount);
        //    if( bids.Sum() < auction.Item.EndPrice) return;

        //    payment.IsDone= true;
        //    if(!await paymentManager.Update(payment)) return;
        //    //send notification to won user
        //    await notificationManager.Add(new Notification
        //    {
        //        Date = DateTime.Now,
        //        Description = $"your Auction {auction.Item.Name} is Done ,Complete The payment",
        //        IsReaded = false,
        //        Title = Enums.NotificationType.auction,
        //        UserId = payment.BuyerId,
        //    });
        //    var lastNotification = notificationManager.GetAll().Where(n => n.UserId == payment.BuyerId).OrderBy(n => n.Id).LastOrDefault();
        //    await notificationsHub.Clients.Group(payment.BuyerId).SendAsync("notification", lastNotification.ToViewModel());


        //    var chat = chatManager.GetAll().Where(c => c.BuyerID == bid.BuyerID && c.SellerID == auction.Item.SellerID);
        //    if (!chat.Any())
        //        await chatManager.Add(new Chat
        //        {
        //            IsActive = false,
        //            BuyerID = bid.BuyerID,
        //            SellerID = auction.Item.SellerID,
        //            StartDate = DateTime.Now,
        //        });

        //    var refundAmount = auction.Item.StartPrice;
        //    var lostBuyersEmails = paymentManager.GetAll().Where(p=>p.AuctionID == _auctionID&&p.IsDone == false).Count();

        //    var result = paymentManager.RefundCustomerAmount("gamal-gamal@personal.example.com", refundAmount * lostBuyersEmails);
        //    //an error here
        //    //if (result.statusCode == 400) return;

        //}

        public async Task EndAuctionAtTime(int auctionID)
        {
            // Retrieve auction details based on the provided auction ID
            var auction = await auctionManager.GetOne(auctionID);
            if (auction == null || auction.Ended) return; 

            // Mark auction as ended and update the auction
            auction.Ended = true;
            if (!await auctionManager.Update(auction)) return;

            // Notify all clients that the auction has ended
            await dashboardHub.Clients.All.SendAsync("endedAuction", auctionID);

            // Retrieve the latest bid for the auction to get the most recent BuyerID
            var latestBid = auction.Bids.OrderByDescending(b => b.Time).FirstOrDefault();
            if (latestBid == null) return;

            // Find the related payment record based on auction ID and latest bid BuyerID
            var payment = paymentManager.GetAll()
                                        .FirstOrDefault(p => p.AuctionID == auctionID && p.BuyerId == latestBid.BuyerID);
            if (payment == null) return;

            //send to seller his auction is ended
            await notificationManager.Add(new Notification
            {
                Date = DateTime.Now,
                Description = $"Your Auction  {auction.Item.Name} is Ended" ,
                IsReaded = false,
                Title = Enums.NotificationType.auction,
                UserId = auction.Item.SellerID
            });
            var lastNotification = notificationManager.GetAll().Where(n=>n.UserId == auction.Item.SellerID).OrderBy(n=>n.Id).LastOrDefault();
            await notificationsHub.Clients.Group(auction.Item.SellerID).SendAsync("notification", lastNotification.ToViewModel());

            // Calculate the total bid amount and verify if it meets the auction's end price
            var totalBidAmount = bidManager.GetAll()
                                           .Where(b => b.AuctionID == auctionID)
                                           .Select(b => b.Amount)
                                           .Sum();
            if (totalBidAmount < auction.Item.EndPrice) return;
            // Update auction with the latest BuyerID and PaymentID
            auction.BuyerID = latestBid.BuyerID;
            auction.PaymentID = payment.Id;
            if (!await auctionManager.Update(auction)) return;

            // Mark the payment as completed and update
            payment.IsDone= true;
            if(!await paymentManager.Update(payment)) return;
            var lastNotify = notificationManager.GetAll().Where(n => n.UserId == payment.BuyerId).OrderBy(n => n.Id).LastOrDefault();
            await notificationsHub.Clients.Group(payment.BuyerId).SendAsync("notification", lastNotify.ToViewModel());
            // Check if a notification already exists for this auction win to avoid duplicate notifications
            var existingNotification = notificationManager.GetAll()
                                                          .Any(n => n.UserId == latestBid.BuyerID && n.Description.Contains($"You won '{auction.Item.Name}'"));
            if (!existingNotification)
            {
                // Send a notification to the latest bidder (winning user)
                var winningNotification = new Notification
                {
                    Date = DateTime.Now,
                    Description = $"You won '{auction.Item.Name}' auction. Please complete the payment.",
                    IsReaded = false,
                    Title = Enums.NotificationType.auction,
                    UserId = latestBid.BuyerID,
                };
                await notificationManager.Add(winningNotification);

                // Retrieve the latest notification for the user and send it in real-time
                var  buyerlastNotification = notificationManager.GetAll()
                                                          .Where(n => n.UserId == latestBid.BuyerID)
                                                          .OrderByDescending(n => n.Id)
                                                          .FirstOrDefault();
                await notificationsHub.Clients.Group(latestBid.BuyerID.ToString()).SendAsync("notification", buyerlastNotification.ToViewModel());
            }

            // Check if a chat already exists for this auction winner to avoid duplicate chats
            var existingChat = chatManager.GetAll()
                                          .Any(c => c.BuyerID == latestBid.BuyerID && c.SellerID == auction.Item.SellerID && !c.IsActive);
            if (!existingChat)
            {
                // Initialize a chat session between the latest buyer and the seller
                await chatManager.Add(new Chat
                {
                    IsActive = false,
                    BuyerID = latestBid.BuyerID,
                    SellerID = auction.Item.SellerID,
                    StartDate = DateTime.Now,
                });
            }

            // Calculate the refund amount for non-winning participants
            var refundAmount = auction.Item.StartPrice;
            var nonWinningBuyersCount = paymentManager.GetAll()
                                                      .Count(p => p.AuctionID == auctionID && !p.IsDone);

            // Process refund for all non-winning buyers
            var refundResult = paymentManager.RefundCustomerAmount("gamal-gamal@personal.example.com", refundAmount * nonWinningBuyersCount);
             await LostAuctionNotifications(auction.ID);
            //if (refundResult.statusCode == 400) return;
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
                .Select(p=>new { p.BuyerId , AuctionId=p.Auction.ID });
            if (!paymentUsersIDs.Any()) return;
            foreach (var payment in paymentUsersIDs)
            {
                var auction = await auctionManager.GetOne(payment.AuctionId);
                var auctionName = auction?.Item?.Name ?? "no auction name";
                await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = $"Sorry you Lost in {auctionName} Auction",
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
