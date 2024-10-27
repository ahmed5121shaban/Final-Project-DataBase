using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;
using Models.Models;
namespace Managers
{
    public class FavAuctionManager:MainManager<FavAuctions>
    {
        public FavAuctionManager(FinalDbContext _context) : base(_context)
        {

        }
    }
}
