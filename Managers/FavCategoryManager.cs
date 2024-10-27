using FinalApi;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class FavCategoryManager:MainManager<FavCategories>
    {
        public FavCategoryManager(FinalDbContext context):base(context)
        {

        }
    }
}
