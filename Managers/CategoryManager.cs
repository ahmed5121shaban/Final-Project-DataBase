using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using ModelView;
using LinqKit;

namespace Managers
{
    
        public class CategoryManager : MainManager<Category>
        {
            public CategoryManager(FinalDbContext context) : base(context)
            {

            }

            public  Pagination<List<Category>> Get(string searchtxt, string calumnName = "Id",
                bool isAscending = false, int pageSize = 2, int PageNumber = 1)
            {
                var builder = PredicateBuilder.New<Category>();
                var old = builder;
                if (!string.IsNullOrEmpty(searchtxt))
                {
                    builder = builder.Or(p => p.Name.Contains(searchtxt));
                }
                if (old == builder)
                {
                    builder = null;
                }

                int total = (builder == null) ? base.GetAll().Count() : base.GetAll().Where(builder).Count();
                var query =  base.Filter(builder, calumnName, isAscending,
                    pageSize, PageNumber);
                return  new Pagination<List<Category>>()
                {
                    PageNumber = PageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    List = query.ToList()
                };

            }

        }
    
}

