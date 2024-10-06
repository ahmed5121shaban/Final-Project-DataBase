using Final;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{

    public class MainManager<T> where T : class
    {
        private readonly FinalDbContext dbContext;

        public MainManager(FinalDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IQueryable<T> GetAll()
        {
            return dbContext.Set<T>().AsQueryable();
        }

        public async Task<T> Get(object _ID)
        {
            return await dbContext.Set<T>().FindAsync(_ID);
        }

        public async Task<bool> Add(T _Item)
        {
            try
            {
                await dbContext.Set<T>().AddAsync(_Item);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> Delete(T _Item)
        {
            try 
            {
                 dbContext.Set<T>().Remove(_Item);
                await dbContext.SaveChangesAsync();
                return true;
            } 
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public async Task Update(T _Item)
        {
            try
            {
                dbContext.Set<T>().Update(_Item);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
