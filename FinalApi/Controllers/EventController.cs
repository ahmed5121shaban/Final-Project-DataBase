using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventManager eventManager;

        public EventController(EventManager _eventManager)
        {
            eventManager = _eventManager;
        }
        [HttpPost]
        public async Task<IActionResult> AddEvent([FromForm] AddEventViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminID))
                return BadRequest(new {message = "the user not found"});
            model.AdminID = adminID;
            if (!await eventManager.Add(model))
                return BadRequest(new {message="the added is not completed"});
            return Created();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            List<EventViewModel> events = await eventManager.GetAll();
            if (events == null)
                return new JsonResult(new ApiResultModel<EventViewModel>
                {
                    Message = "no result found",
                    result=null,
                    StatusCode = 404,
                    success=false
                });
            return new JsonResult(new ApiResultModel<List<EventViewModel>>
            {
                Message = "result returned successfully.",
                result = events,
                StatusCode = 200,
                success = true
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if(!await eventManager.Delete(id))
                return BadRequest(new {message="the event not found"});
            return Ok();
        }


    }
}
