using Final;
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
    }
}
