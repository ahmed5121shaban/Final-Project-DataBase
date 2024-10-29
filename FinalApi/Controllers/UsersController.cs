using FinalApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ModelView;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager _userManager;

        public UsersController(UserManager userManager)
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
        public async Task<IActionResult> GetUsers([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userManager.GetFilteredUsersAsync(search, page, pageSize);
            return Ok(result);
        }
    }
}
