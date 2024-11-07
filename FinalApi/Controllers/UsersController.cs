using FinalApi;
using Microsoft.AspNetCore.Mvc;
using ModelView;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CustomUserManager _userManager;

        public UsersController(CustomUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.GetUserWithRoleByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _userManager.GetFilteredUsersAsync(search, page, pageSize);
            return Ok(result);
        }
    }
}
