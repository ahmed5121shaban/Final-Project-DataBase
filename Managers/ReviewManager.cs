using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class ReviewManager :MainManager<Review>
    {
        public ReviewManager(FinalDbContext context):base(context)
        { 
        }
    }
}
