using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class AuctionManager:MainManager<Auction>
    {
        public AuctionManager(FinalDbContext _dbContext) :base(_dbContext)
        {

        }
    }
}
