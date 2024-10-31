using System.Collections.Generic;

namespace ModelView
{
    public class PaginatedUsersResult
    {
        public List<UserViewModel> Users { get; set; }
        public int TotalUsers { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
