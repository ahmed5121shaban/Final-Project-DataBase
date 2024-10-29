using FinalApi;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Models;
using ModelView;

namespace Managers
{
    public class ComplainManager : MainManager<Complain>
    {
        private readonly FinalDbContext _dbContext;

        public ComplainManager(FinalDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        // إضافة شكوى
        public async Task<bool> AddComplain(ComplainAddViewModel model)
        {
            // التحقق من أن الدفع تم (IsDone) والتأكد من SellerID عبر Item
            var payment = await _dbContext.Payment
                .Include(p => p.Auction) // تضمين المزاد المرتبط بالدفع
                .ThenInclude(a => a.Item) // تضمين الـ Item المرتبط بالمزاد
                .FirstOrDefaultAsync(p => p.BuyerId == model.BuyerID && p.IsDone && p.Auction.Item.SellerID == model.SellerID);

            if (payment == null)
            {
                // لم يتم العثور على دفع مكتمل
                return false;
            }

            var complain = new Complain
            {
                Reason = model.Reason,
                BuyerID = model.BuyerID,
                SellerID = payment.Auction.Item.SellerID // استخدام SellerID من الـ Item
            };

            return await Add(complain);  // إضافة الشكوى
        }

        // الحصول على الشكاوى
        public async Task<Pagination<List<ComplainDisplayViewModel>>> GetComplains(int pageNumber, int pageSize, string searchText = "")
        {
            var query = _dbContext.Complains.AsQueryable();

            // إضافة الفلترة حسب النص
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => c.Reason.Contains(searchText));
            }

            var totalCount = await query.CountAsync();

            // تأكد من أن النتيجة هي قائمة (List)
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
                    BuyerName = c.Buyer.User.Name
                })
                .ToListAsync();  // تحويل النتيجة إلى قائمة

            // إرجاع القائمة داخل الكائن Pagination
            return new Pagination<List<ComplainDisplayViewModel>>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                List = complainsList  // هنا استخدام complainsList
            };
        }

        public async Task<List<SellerViewModel>> GetSellersByBuyerId(int buyerId)
        {
            var sellers = await _dbContext.Payment
                .Include(p => p.Auction) // تضمين المزاد المرتبط بالدفع
                .ThenInclude(a => a.Item) // تضمين الـ Item المرتبط بالمزاد
                .Where(p => p.BuyerId == buyerId.ToString() && p.IsDone) // تحويل buyerId إلى string
                .GroupBy(p => p.Auction.Item.SellerID) // تجميع حسب SellerID
                .Select(g => new SellerViewModel
                {
                    Id = int.Parse(g.Key), // تحويل SellerID إلى int
                    Name = g.First().Auction.Item.Seller.User.Name // تأكد من أن Seller.User.Name موجود
                })
                .ToListAsync();

            return sellers;
        }



    }
}
