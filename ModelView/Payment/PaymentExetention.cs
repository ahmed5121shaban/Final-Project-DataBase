using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public static class PaymentExetention
    {
        public static Payment ToModel(this PaymentViewModel _paymentView)
        {
            return new Payment()
            {
                BuyerId = _paymentView.BuyerId,
                AuctionID = (int)_paymentView.AuctionID,
                Method = _paymentView.Method,
                IsDone = false,
            };
        }
        
        public static Payment ToModel(this AddPaymentViewModel _paymentView)
        {
            return new Payment()
            {
                BuyerId = _paymentView.BuyerId,
                Method = _paymentView.Method,
                IsDone = false,
            };
        }
        public static Payment ToModel(this PaymentStartPriceViewModel _paymentView)
        {
            return new Payment()
            {
                BuyerId = _paymentView.BuyerID,
                AuctionID = _paymentView.AuctionID,
                Method = _paymentView.Method,
                IsDone = false,
            };
        }

    }
}
