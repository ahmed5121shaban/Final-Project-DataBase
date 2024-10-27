using FinalApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelView;
using PayPal.Api;
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
        private readonly UserManager<User> userManager;
        private readonly BuyerManager buyerManager;

        public BidManager(FinalDbContext context,PaymentManager _paymentManager,
            AuctionManager _auctionManager,UserManager<User> _userManager,BuyerManager _buyerManager) : base(context)
        {
            paymentManager = _paymentManager;
            auctionManager = _auctionManager;
            userManager = _userManager;
            buyerManager = _buyerManager;
        }

        public Bid GetHighest(int AuctionId)
        {
            var all =  base.GetAll().Where(i => i.AuctionID == AuctionId);
            var highestBid =  all.OrderByDescending(b => b.Amount).FirstOrDefault();

            return highestBid;
        }
 
        public async Task<string> MinceAuctionStartPrice(PaymentStartPriceViewModel _paymentView)
        {
            var Bid = base.GetAll().FirstOrDefault(u => u.BuyerID == _paymentView.BuyerID&&u.AuctionID==_paymentView.AuctionID);
            if (Bid != null)
                return string.Empty;
               string Email = string.Empty;
                if (_paymentView.Method == Enums.PaymentMetod.paypal)
                {
                    string result = paymentManager.AddPayPalPayment(new CreatePaymentViewModel { Amount = _paymentView.Amount, Currency = _paymentView.Currency,auctionID = _paymentView.AuctionID });
                    if (string.IsNullOrEmpty(result))
                        return string.Empty;
                    Email = _paymentView.PayPalEmail;
                    var res = await paymentManager.Add(_paymentView.ToModel());
                    return result;
                }
                else
                {
                    string result = paymentManager.AddStripePayment(new CreatePaymentViewModel { Amount = _paymentView.Amount, Currency = _paymentView.Currency, auctionID = _paymentView.AuctionID });
                    if (string.IsNullOrEmpty(result))
                        return string.Empty;
                    Email = _paymentView.StripeEmail;
                    var res = await paymentManager.Add(_paymentView.ToModel());
                    return result;
                }
        }
    }
}
