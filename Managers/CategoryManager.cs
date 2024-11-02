using FinalApi;
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

            public  Pagination<List<CategoryViewModel>> Get(string searchtxt)
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
                var query =  base.Filter(builder);

                return  new Pagination<List<CategoryViewModel>>()
                {
                   
                    TotalCount = total,
                    List = query.Select(c=>c.ToViewModel()).ToList()
                };

            }

        }
    
}

