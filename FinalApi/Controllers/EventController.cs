using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ModelView;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace FinalApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventManager eventManager;
        private readonly ItemManager itemManager;
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly NotificationManager notificationManager;

        public EventController(EventManager _eventManager,ItemManager _itemManager,
            IHubContext<NotificationsHub> _hubContext,NotificationManager _notificationManager)
        {
            eventManager = _eventManager;
            itemManager = _itemManager;
            hubContext = _hubContext;
            notificationManager = _notificationManager;
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
           
            List<string> ids = new List<string>();
            for (int i = 0; i < model.itemsID.Length; i++)
            {
                if (model.itemsID[i] != '-')
                {
                    var item = await itemManager.Get(int.Parse(model.itemsID[i].ToString()));
                    if (item != null)
                    {
                        ids.Add(item.SellerID);
                    }
                }
            }

            foreach (var id in ids)
            {
                await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = "your item added to event",
                    IsReaded = false,
                    Title = Enums.NotificationType.events,
                    UserId = id
                });
                Notification lastNotification = notificationManager.GetAll().Where(n=>n.UserId== id).OrderBy(n => n.Id).LastOrDefault();
                if (lastNotification == null)
                    return BadRequest(new { message = "no last notification found" });

                await hubContext.Clients.Groups(id).SendAsync("notification", lastNotification.ToViewModel());
            }


            return Created();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var Event = await eventManager.GetAll();
            if (Event == null)
                return new JsonResult(new ApiResultModel<string>
                {
                    Message = "no result found",
                    result="",
                    StatusCode = 404,
                    success=false
                });
            return new JsonResult(new ApiResultModel<List<EventViewModel>>
            {
                Message = "result returned successfully.",
                result = Event,
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

        [HttpGet("{_id:int}")]
        public async Task<IActionResult> GetEvent(int _id)
        {
            var eventResult =await eventManager.Get(_id);
            if (eventResult == null)
                return BadRequest(new {Message="no event found with this id"});

            return Ok(eventResult.ToEventViewModel());
        }

        [HttpGet("home-event")]
        public async Task<IActionResult> GetHomeEvent()
        {
            var _event = eventManager.GetAllEvent().Result.OrderBy(e=>e.ID).LastOrDefault();
            if (_event == null) return new JsonResult
            (new ApiResultModel<string>{
                Message = "the last event not found",
                result = null,
                StatusCode =404,
                success = false
            });
            return new JsonResult
            (new ApiResultModel<EventViewModel>
            {
                Message = "the data is completed",
                result = _event.ToViewModel(),
                StatusCode = 200,
                success = true
            });
        }
    }
}
