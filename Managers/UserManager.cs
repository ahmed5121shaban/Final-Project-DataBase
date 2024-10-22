using Final;
using Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // تأكد من إضافته
using ModelView;
using ModelView.Users;

public class UserManager : MainManager<User>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManager(FinalDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        : base(dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<PaginatedUsersResult> GetFilteredUsersAsync(string search, int page, int pageSize)
    {
        var query = _userManager.Users.AsQueryable();
        return await query.ApplyFilterAndPaginationAsync(search, page, pageSize, _userManager);
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
