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
 
        public async Task<string> MinceAuctionStartPrice(PaymentStartPriceViewModel _paymentView)
        {
            var res = base.GetAll().FirstOrDefault(u => u.BuyerID == _paymentView.BuyerID&&u.AuctionID==_paymentView.AuctionID);
            if (res != null)
                return string.Empty;
               string Email = string.Empty;
                if (_paymentView.Method == Enums.PaymentMetod.paypal)
                {
                    string result = paymentManager.AddPayPalPayment(new CreatePaymentViewModel { Amount = _paymentView.Amount, Currency = _paymentView.Currency,auctionID = _paymentView.AuctionID });
                    if (string.IsNullOrEmpty(result))
                    return string.Empty;
                
                    Email = _paymentView.PayPalEmail;
                    var payment = paymentManager.GetAll().FirstOrDefault(p => p.Method == Enums.PaymentMetod.paypal && p.BuyerId == _paymentView.BuyerID);
                    if (payment != null)
                    {
                        payment.AuctionID = _paymentView.AuctionID;
                        await paymentManager.Update(payment);
                    }
                return result;


            }
                else
                {
                    string result = paymentManager.AddStripePayment(new CreatePaymentViewModel { Amount = _paymentView.Amount, Currency = _paymentView.Currency, auctionID = _paymentView.AuctionID });
                    if (string.IsNullOrEmpty(result))
                    return string.Empty;
                    Email = _paymentView.StripeEmail;
                    var payment = paymentManager.GetAll().FirstOrDefault(p => p.Method == Enums.PaymentMetod.stripe && p.BuyerId == _paymentView.BuyerID);
                    if (payment != null)
                    {
                        payment.AuctionID = _paymentView.AuctionID;
                        await paymentManager.Update(payment);
                    }
                return result;
            }


        }
    }
}
