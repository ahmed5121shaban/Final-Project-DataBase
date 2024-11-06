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
        private readonly FavCategoryManager favCategoryManager;
        private readonly ChatManager chatManager;
        private readonly FavAuctionManager favAuctionManager;
        private readonly IHubContext<DashboardHub> dashboardHub;

        public HangfireManager(AuctionManager _auctionManager, PaymentManager _paymentManager,
            BidManager _bidManager, IHubContext<NotificationsHub> _notificationsHub,
            IHubContext<DashboardHub> _dashboardHub,
            NotificationManager _notificationManager, FavCategoryManager _favCategoryManager,
            ChatManager _chatManager, FavAuctionManager _favAuctionManager)
        {
            auctionManager = _auctionManager;
            paymentManager = _paymentManager;
            bidManager = _bidManager;
            notificationsHub = _notificationsHub;
            notificationManager = _notificationManager;
            favCategoryManager = _favCategoryManager;
            chatManager = _chatManager;
            favAuctionManager = _favAuctionManager;
            dashboardHub = _dashboardHub;
        }

        public async Task EndAuctionAtTime(int auctionID)
        {
            // Retrieve auction details based on the provided auction ID
            var auction = await auctionManager.GetOne(auctionID);
            if (auction == null) return;

            // Mark auction as ended and update the auction
            auction.Ended = true;
            if (!await auctionManager.Update(auction)) return;

            // Notify all clients that the auction has ended
            await notificationManager.Add(new Notification
            {
                Date = DateTime.Now,
                Description = $"Your Auction  {auction.Item.Name} is Ended",
                IsReaded = false,
                Title = Enums.NotificationType.auction,
                UserId = auction.Item.SellerID
            });
            var lastNotification = notificationManager.GetAll().Where(n => n.UserId == auction.Item.SellerID).OrderBy(n => n.Id).LastOrDefault();
            await notificationsHub.Clients.Group(auction.Item.SellerID).SendAsync("notification", lastNotification.ToViewModel());
            await dashboardHub.Clients.All.SendAsync("endedAuction", auctionID);

            // Retrieve the latest bid for the auction to get the most recent BuyerID
            var latestBid = auction.Bids.OrderByDescending(b => b.Time).FirstOrDefault();
            if (latestBid == null) return;

            // Find the related payment record based on auction ID and latest bid BuyerID
            var payment = paymentManager.GetAll()
                                        .FirstOrDefault(p => p.AuctionID == auctionID && p.BuyerId == latestBid.BuyerID);
            if (payment == null) return;



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
            payment.IsDone = true;
            if (!await paymentManager.Update(payment)) return;


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
            //send to buyer to complete his payment
            await notificationManager.Add(new Notification
            {
                Date = DateTime.Now,
                Description = $"You won '{auction.Item.Name}' auction. Please complete the payment.",
                IsReaded = false,
                Title = Enums.NotificationType.auction,
                UserId = latestBid.BuyerID,
            });
            var buyerlastNotification = notificationManager.GetAll()
                                                          .Where(n => n.UserId == latestBid.BuyerID)
                                                          .OrderByDescending(n => n.Id)
                                                          .FirstOrDefault();
            await notificationsHub.Clients.Group(latestBid.BuyerID).SendAsync("notification", buyerlastNotification.ToViewModel());




            // Calculate the refund amount for non-winning participants
            var refundAmount = auction.Item.StartPrice;
            var nonWinningBuyersCount = paymentManager.GetAll()
                                                      .Count(p => p.AuctionID == auctionID && !p.IsDone);

            // Process refund for all non-winning buyers
            var refundResult = paymentManager.RefundCustomerAmount("gamal-gamal@personal.example.com", refundAmount * nonWinningBuyersCount);
            //if (refundResult.statusCode == 400) return;

            //sent to all lost buyers

            var paymentUsersIDs = paymentManager.GetAll().Where(p => p.AuctionID == auctionID && p.IsDone == false)
             .Select(p => p.BuyerId).ToList();
            if (!paymentUsersIDs.Any()) return;
               foreach (var pay in paymentUsersIDs)
               {
                await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = $"Sorry you Lost in {auction.Item.Name} Auction",
                    IsReaded = false,
                    Title = Enums.NotificationType.auction,
                    UserId = pay,
                });
                var lostNotification = notificationManager.GetAll().Where(n => n.UserId == pay).OrderBy(n => n.Id)
                    .LastOrDefault();
                await notificationsHub.Clients.Groups(pay).SendAsync("notification", lostNotification.ToViewModel());
               }

            
        }


        public async Task AuctionEndedNotificationBeforeOneDay(int _auctionID)
        {
            var favAuctions = favAuctionManager.GetAll().Where(f => f.AuctionID == _auctionID);
            if (!favAuctions.Any()) return;
            foreach (var fav in favAuctions)
            {
                await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = $"This Auction {fav.Auction.Item.Name} have One Day To End",
                    IsReaded = false,
                    Title = Enums.NotificationType.auction,
                    UserId = fav.BuyerID,
                });

                Notification lastNotification = notificationManager.GetAll().Where(n => n.UserId == fav.BuyerID).OrderBy(n => n.Id).LastOrDefault();

                await notificationsHub.Clients.Groups(fav.BuyerID).SendAsync("notification", lastNotification.ToViewModel());
            }

        }


        public async Task SendNotificationsToUserInFavCategory(int _categoryID, int _auctionID)
        {
            var favCatDetail = favCategoryManager.GetAll().Where(f => f.CategoryID == _categoryID)
                .Select(f => new { buyerID = f.BuyerID, categoryName = f.Category.Name }).ToList();
            if (!favCatDetail.Any()) return;

            foreach (var id in favCatDetail)
            {
                if (await notificationManager.Add(new Notification
                {
                    Title = Enums.NotificationType.auction,
                    UserId = id.buyerID,
                    Date = DateTime.Now,
                    Description = $"New Auction Added in your Favorite Category : {id.categoryName}",
                    IsReaded = false,
                }))
                {
                    try
                    {
                        var lastNotification = notificationManager.GetAll().Where(n => n.UserId == id.buyerID).OrderBy(n => n.Id).LastOrDefault();
                        if (lastNotification == null)
                            return;

                        await notificationsHub.Clients.Groups(id.buyerID).SendAsync("notification", lastNotification.ToViewModel());
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }

        }
    }
}