using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ModelView;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        private readonly AccountManager acountManager;

        public AcountController(AccountManager _acountManager)
        {
            acountManager = _acountManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel _loginView)
        {
            if (!ModelState.IsValid) return BadRequest(new {Message="This Data Is Not Completed"}) ;
            var res = await acountManager.Login(_loginView);
            if (res==null) return BadRequest(new { Message = "Error In Login Operation" }); 
            return Ok(new {token=res,exepire=DateTime.Now.AddDays(30),status=200});
        }

        [HttpPost("register")]
        public async Task< IActionResult> Register([FromBody]RegisterViewModel _registerView)
        {
            try
            {

            if (!ModelState.IsValid) return BadRequest(new {Message= "This Data Is Not Completed" });
            var res = await acountManager.Register(_registerView);
            if (!res.Succeeded) return BadRequest(new { Message = "Error In Register Operation" });
            return Ok(new { status = 200 });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = "Error In Register Operation" });
            }
        }
    }
}
