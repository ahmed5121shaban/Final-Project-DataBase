using FinalApi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{

    public class MainManager<T> where T : class
    {
        private readonly FinalDbContext dbContext;
        private DbSet<T> dbSet;
        public MainManager(FinalDbContext _dbContext)
        {
            dbContext = _dbContext;
            dbSet = dbContext.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return dbSet.AsQueryable();
        }

        public async Task<T> GetOne(object _ID)
        {
            return await dbSet.FindAsync(_ID);
        }
        public async Task<T> Get(object _ID)
        {
            return await dbSet.FindAsync(_ID);
        }

        public async Task<bool> Add(T _Item)
        {
            try
            {
                await dbSet.AddAsync(_Item);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
              
                return false;
            }

        }

        public async Task<bool> Delete(T _Item)
        {
            try
            {
                dbSet.Remove(_Item);
                await dbContext.SaveChangesAsync();
                return true;
            } 
            catch (Exception ex) {  
                  return false;
            }
        }

        public async Task<bool> Update(T _Item)
        {
            try
            {
                dbSet.Update(_Item);
                await dbContext.SaveChangesAsync();
                return true;

            }
            catch
            {
                //throw ex;
                return false;
            }




        }

        public IQueryable<T> Filter(Expression<Func<T, bool>> expression, string calumnName = "Id", bool isAscending = false, int pageSize = 6, int pageNumber = 1)
        {
            var query = dbSet.AsQueryable();
            if (expression != null)
            {
                query = dbSet.Where(expression);
            }
            if (!string.IsNullOrEmpty(calumnName))
            {
                query = query.OrderBy(calumnName, isAscending);
            }
            if (pageSize < 0)
            {
                pageSize = 6;
            }
            if (pageNumber < 0)
            {
                pageNumber = 1;
            }
            int skipcount = (pageNumber - 1) * pageSize;
            query = query.Skip(skipcount).Take(pageSize);

            return query;
        }

    }
}
