using FinalApi;
using Microsoft.AspNetCore.Identity;
using ModelView;


namespace Managers
{
    public class SellerManager:MainManager<Seller>
    {
        readonly FinalDbContext context;
        public SellerManager(FinalDbContext _context):base(_context) {
        this.context=_context;
        }  
    }
}
