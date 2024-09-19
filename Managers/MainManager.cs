using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class MainManager<T> where T : class
    {
        public IQueryable<T> GetAll()
        {
            return null;
        }

        public T Get(object _ID)
        {
            return null;
        }

        public bool Add(T _Item)
        {
            return false;
        }

        public bool Delete(T _Item)
        {
            return false;
        }

        public bool Update(T _Item)
        {
            return false;
        }
    }
}
