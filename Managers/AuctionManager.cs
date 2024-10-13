using Final;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Managers
{
    public class AuctionManager:MainManager<Auction>
    {
        private readonly FinalDbContext dbcontext;
        private DbSet<Auction> dbset;
        public AuctionManager(FinalDbContext _dbContext) :base(_dbContext)
        {
            dbcontext = _dbContext;
            dbset = dbcontext.Set<Auction>();
        }

        //public Pagination<List<Auction>> Get(string searchtxt, string calumnName = "Id",
        //    bool isAscending = false, int pageSize = 2, int PageNumber = 1)
        //{
        //    var builder = PredicateBuilder.New<Auction>();
        //    var old = builder;
        //    if (!string.IsNullOrEmpty(searchtxt))
        //    {
        //        builder = builder.Or(p => p.Item.Name.Contains(searchtxt));
        //    }
        //    if (old == builder)
        //    {
        //        builder = null;
        //    }

        //    int total = (builder == null) ? base.GetAll().Count() : base.GetAll().Where(builder).Count();
        //    var query = base.Filter(builder, calumnName, isAscending,
        //        pageSize, PageNumber);
        //    return new Pagination<List<Auction>>()
        //    {
        //        PageNumber = PageNumber,
        //        PageSize = pageSize,
        //        TotalCount = total,
        //        List = query.ToList()
        //    };

        //}

        public Pagination<List<Auction>> Get(string searchtxt, string columnName = "Id",
    bool isAscending = false, int pageSize = 2, int PageNumber = 1, string? categoryName = null)
        {
            var builder = PredicateBuilder.New<Auction>();
            var old = builder;

            // Filter by search text (if provided)
            if (!string.IsNullOrEmpty(searchtxt))
            {
                builder = builder.Or(p => p.Item.Name.Contains(searchtxt));
            }

            // Filter by category (if provided)
            if (!string.IsNullOrEmpty(categoryName))
            {
                builder = builder.And(p => p.Item.Category.Name == categoryName);
            }

            if (old == builder)
            {
                builder = null;  // No filters applied
            }

            // Count total filtered items
            int total = (builder == null) ? base.GetAll().Count() : base.GetAll().Where(builder).Count();

            // Apply filter, sorting, and pagination
            var query = base.Filter(builder, columnName, isAscending, pageSize, PageNumber);

            // Return paginated result
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
