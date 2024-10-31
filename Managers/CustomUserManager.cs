using FinalApi;
using Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelView;
using System.Linq;
using System.Threading.Tasks;

public class CustomUserManager : MainManager<User>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CustomUserManager(FinalDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        : base(dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<PaginatedUsersResult> GetFilteredUsersAsync(string search, int page, int pageSize)
    {
        var query = _userManager.Users.AsQueryable();

        // Apply any filtering logic here if needed, then paginate.
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.UserName.Contains(search) || u.Email.Contains(search));
        }

        // Pagination logic
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Map users to view models
        var result = new PaginatedUsersResult
        {
            Users = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "No Role"
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = await query.CountAsync()
        };

        return result;
    }

    public async Task<UserViewModel> GetUserWithRoleByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = roles.FirstOrDefault() ?? "No Role"
        };
    }
}
