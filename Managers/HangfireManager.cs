using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class HangfireManager
    {
        private readonly AuctionManager auctionManager;
        private readonly PaymentManager paymentManager;
        private readonly BidManager bidManager;

        public HangfireManager(AuctionManager _auctionManager,PaymentManager _paymentManager,BidManager _bidManager)
        {
            auctionManager = _auctionManager;
            paymentManager = _paymentManager;
            bidManager = _bidManager;
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

            //refund the payment to all lost buyer

            //when the auction done (the time end with buyer bid more than expected price) i must add the buyer id and payment id
            auction.BuyerID = payment.BuyerId;
            auction.PaymentID = payment.Id; 
            await auctionManager.Update(auction);

        }
    }
}
