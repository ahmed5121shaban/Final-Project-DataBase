using FinalApi;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Models;
using ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FinalApi.Enums;

namespace Managers
{
    public class ComplainManager : MainManager<Complain>
    {
        private readonly FinalDbContext _dbContext;

        public ComplainManager(FinalDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddComplain(ComplainAddViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.BuyerID))
            {
                Console.WriteLine("BuyerID is required but was not provided.");
                return false;
            }

            var payment = await _dbContext.Payment
                .Include(p => p.Auction)
                .ThenInclude(a => a.Item)
                .FirstOrDefaultAsync(p =>
                    p.BuyerId == model.BuyerID &&
                    p.IsDone &&
                    p.Auction.Item.SellerID == model.SellerID.ToString() &&
                    p.Auction.ShippingStatus == AuctionShippingStatus.Returned
                );

            if (payment == null)
            {
                Console.WriteLine("No completed payment found or conditions not met for the auction.");
                return false;
            }

            var complain = new Complain
            {
                Reason = model.Reason,
                BuyerID = model.BuyerID,
                SellerID = payment.Auction.Item.SellerID.ToString()
            };

            return await Add(complain);
        }

        public async Task<Pagination<List<ComplainDisplayViewModel>>> GetComplains(int pageNumber, int pageSize, string searchText = "")
        {
            var query = _dbContext.Complains.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => c.Reason.Contains(searchText));
            }

            var totalCount = await query.CountAsync();

            var complainsList = await query
                .Include(c => c.Seller)
                .Include(c => c.Buyer)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ComplainDisplayViewModel
                {
                    Id = c.ID,
                    Reason = c.Reason,
                    SellerName = c.Seller.User.Name,
                    BuyerName = c.Buyer.User.Name,
                    selleremail = c.Seller.User.Email,
                    buyeremail = c.Buyer.User.Email
                })
                .ToListAsync();

            return new Pagination<List<ComplainDisplayViewModel>>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                List = complainsList
            };
        }

        public async Task<List<SellerViewModel>> GetSellersByBuyerId(string buyerId)
        {
            if (string.IsNullOrWhiteSpace(buyerId))
            {
                Console.WriteLine("BuyerID is required to fetch sellers.");
                return new List<SellerViewModel>();
            }

            var query = _dbContext.Payment
                .Include(p => p.Auction)
                .ThenInclude(a => a.Item)
                .ThenInclude(i => i.Seller)
                .Where(p => p.BuyerId == buyerId && p.IsDone)
                .GroupBy(p => p.Auction.Item.Seller.UserID)
                .Select(g => new
                {
                    SellerId = g.Key,
                    Name = g.Select(x => x.Auction.Item.Seller.User.Name).FirstOrDefault()
                });

            var sellersData = await query.ToListAsync();

            var sellers = sellersData.Select(x => new SellerViewModel
            {
                Id = x.SellerId,
                Name = x.Name
            }).ToList();

            return sellers;
        }
    }
}
