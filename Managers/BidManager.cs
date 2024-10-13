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

        public BidManager(FinalDbContext context,PaymentManager _paymentManager) : base(context)
        {
            paymentManager = _paymentManager;
        }

        public Bid GetHighest(int AuctionId)
        {
            var all =  base.GetAll().Where(i => i.AuctionID == AuctionId);
            var highestBid =  all.OrderByDescending(b => b.Amount).FirstOrDefault();

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
