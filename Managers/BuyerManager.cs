﻿using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final;
namespace Managers
{
    public class BuyerManager:MainManager<Buyer>
    {
        private FinalDbContext context;
        public BuyerManager(FinalDbContext _context) : base(_context)
        {
            this.context = _context;
        }
    }
}
