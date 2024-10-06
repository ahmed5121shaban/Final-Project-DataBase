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
       
        public BidManager(FinalDbContext context) : base(context)
        {

        }
        public Bid GetHighest(int AuctionId)
        {
            var all =  base.GetAll().Where(i => i.AuctionID == AuctionId);
            var highestBid =  all.OrderByDescending(b => b.Amount).FirstOrDefault();

            return highestBid;
        }
    }
}
