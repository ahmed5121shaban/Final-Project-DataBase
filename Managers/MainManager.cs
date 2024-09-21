using Final;
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

        public T Get(object _ID)
        {
            return dbContext.Set<T>().Find(_ID);
        }

        public bool Add(T _Item)
        {
            try
            {
                dbContext.Set<T>().Add(_Item);
                return true;
            }
            catch (Exception ex) 
            {
               throw ex.InnerException;
            }
            
        }

        public bool Delete(T _Item)
        {
            try 
            { 
                dbContext.Set<T>().Remove(_Item);
                return true;
            } 
            catch (Exception ex) 
            {
                throw ex.InnerException;
            }
        }

        public bool Update(T _Item)
        {
            try
            {
                dbContext.Set<T>().Update(_Item);
                return true;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
