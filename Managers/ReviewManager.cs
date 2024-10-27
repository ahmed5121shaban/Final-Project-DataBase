using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace Managers
{
    public class ReviewManager:MainManager<Review>
    {
        public ReviewManager(FinalDbContext _context) : base(_context)
        {

        }
    }
}
