using FinalApi;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Managers
{
    public class AuctionManager : MainManager<Auction>
    {
        private readonly FinalDbContext dbcontext;
        private DbSet<Auction> dbset;
        public AuctionManager(FinalDbContext _dbContext) : base(_dbContext)
        {
            dbcontext = _dbContext;
            dbset = dbcontext.Set<Auction>();
        }

        public async Task<Auction> Add(Auction auction)
        {
            try
            {
                dbset.Add(auction);
                await dbcontext.SaveChangesAsync();
                return auction;
            }catch(Exception ex)
            {
                return null;
            }
           
        }



        public Pagination<List<Auction>> Get(
    string searchtxt,
    string columnName = "Id",
    bool isAscending = false,
    int pageSize = 2,
    int pageNumber = 1,
    string? categoryName = null,
    string? filterOption = null
)
        {
            var builder = PredicateBuilder.New<Auction>(true); 
            var old = builder;

            // Filter by search text (if provided)
            if (!string.IsNullOrEmpty(searchtxt))
            {
                builder = builder.And(p => p.Item.Name.Contains(searchtxt));
            }

            // Filter by category (if provided)
            if (!string.IsNullOrEmpty(categoryName))
            {
                builder = builder.And(p => p.Item.Category.Name == categoryName);
            }

            // Handle filter options
            if (filterOption == "EndDate")
            {
                columnName = "EndDate";
                isAscending = true;
            }
            else if (filterOption == "mostBids")
            {
                builder = builder.And(p => p.Bids.Count() > 0);
                isAscending = false; // Sort in descending order to get the most bids
            }
            else if (filterOption == "highestBid")
            {
                builder = builder.And(p => p.Bids.Any());
                isAscending = false;
            }
            else if (filterOption == "lowestBid")
            {
                // Ensure we only include auctions that have bids
                builder = builder.And(p => p.Bids.Any());
                isAscending = true; // We want the lowest bid
            }

            else if (filterOption == "newArrivals")
            {
                builder = builder.And(p => p.StartDate >= DateTime.Now.AddDays(-5));
                isAscending = false; // We want the lowest bid
            }

            else if (filterOption == "noBids")
            {
                builder = builder.And(p => p.Bids.Count()==0);
                isAscending = false; // We want the lowest bid
            }

            // If no filters were applied
            if (old == builder)
            {
                builder = null;  // No filters applied
            }

            // Count total filtered items
            int total = (builder == null) ? base.GetAll().Count() : base.GetAll().Where(builder).Count();

            // Apply filter and pagination
            var query = base.Filter(builder, columnName, isAscending, pageSize, pageNumber);

            // Apply sorting based on filter options
            if (filterOption == "mostBids")
            {
                query = query.OrderByDescending(a => a.Bids.Count());
            }
            else if (filterOption == "highestBid")
            {
                query = query.OrderByDescending(a => a.Bids.Max(b => b.Amount));
            }
            else if (filterOption == "lowestBid")
            {
                query = query.OrderBy(a => a.Bids.Min(b => b.Amount));
            }
            else if (filterOption == "newArrivals")
            {
                query = query.OrderByDescending(a => a.StartDate);
            }

            // Return paginated result
            return new Pagination<List<Auction>>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                List = query.ToList()
            };
        }

    }
}
