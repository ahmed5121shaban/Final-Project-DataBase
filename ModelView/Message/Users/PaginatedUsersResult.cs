using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class PaginatedUsersResult
    {
        public int TotalUsers { get; set; }
        public List<UserViewModel> Users { get; set; }
    }

}
