using Final;
using LinqKit;
using Models;
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
      
        public Pagination<List<Auction>> Get(string searchtxt, string calumnName = "Id",
            bool isAscending = false, int pageSize = 2, int PageNumber = 1)
        {
            var builder = PredicateBuilder.New<Auction>();
            var old = builder;
            if (!string.IsNullOrEmpty(searchtxt))
            {
                builder = builder.Or(p => p.Item.Name.Contains(searchtxt));
            }
            if (old == builder)
            {
                builder = null;
            }

            int total = (builder == null) ? base.GetAll().Count() : base.GetAll().Where(builder).Count();
            var query = base.Filter(builder, calumnName, isAscending,
                pageSize, PageNumber);
            return new Pagination<List<Auction>>()
            {
                PageNumber = PageNumber,
                PageSize = pageSize,
                TotalCount = total,
                List = query.ToList()
            };

        }
    }
}
