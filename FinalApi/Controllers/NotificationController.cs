using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationManager notificationManager;

        public NotificationController(NotificationManager _notificationManager)
        {
            notificationManager = _notificationManager;
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "user not found" });
            List<NotificationViewModel> notificationViewModels = new List<NotificationViewModel>();
            var notifications = notificationManager.GetAll().Where(n => n.UserId == userId).ToList();
            if (notifications.Any())
            {
                foreach (var item in notifications)
                    notificationViewModels.Add(item.ToViewModel());
            }
               

            return new JsonResult(new ApiResultModel<List<NotificationViewModel>>
            {
                result = notificationViewModels,
                Message = "the data completed",
                StatusCode = 200,
                success = true
            });
        }
    }
}
