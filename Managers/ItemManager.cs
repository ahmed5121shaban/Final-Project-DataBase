using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class ItemManager : MainManager<Item>
    {
        public ItemManager(FinalDbContext _context) : base(_context)
        {

        }

    }
}
