using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class BuyerManager : MainManager<Buyer>
    {
        public BuyerManager(FinalDbContext _dbContext) : base(_dbContext)
        {
        }
    }
}
