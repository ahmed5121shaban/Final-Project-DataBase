using Final;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class BidManager :MainManager<Bid>
    {
        private readonly PaymentManager paymentManager;
        private readonly AuctionManager auctionManager;

        public BidManager(FinalDbContext context,PaymentManager _paymentManager,AuctionManager _auctionManager) : base(context)
        {
            paymentManager = _paymentManager;
            auctionManager = _auctionManager;
        }

        public Bid GetHighest(int AuctionId)
        {
            var allBids =  base.GetAll().Where(i => i.AuctionID == AuctionId);
            var highestBid = allBids.OrderByDescending(b => b.Amount).FirstOrDefault();
            //if (highestBid == null)
            //{
            //    var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == AuctionId);
            //    highestBid.Amount = auction.Item.StartPrice; 
                
            //}

            return highestBid;
        }

        public bool MinceAuctionStartrice(PaymentViewModel _paymentView)
        {

            if (_paymentView.Method == Enums.PaymentMetod.paypal)
                return paymentManager.AddPayPalPayment(_paymentView); 
            
            return paymentManager.AddPayPalPayment(_paymentView);

        }
    }
}
