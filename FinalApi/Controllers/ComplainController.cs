using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Managers;
using System.Threading.Tasks;
using ModelView.Complain;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using FinalApi;
using FinalApi;
using ModelView;
using System.Security.Claims;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplainController : ControllerBase
    {
        private readonly ComplainManager _complainManager;
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly NotificationManager notificationManager;

        public ComplainController(ComplainManager complainManager,IHubContext<NotificationsHub> _hubContext,
            NotificationManager _notificationManager)
        {
            _complainManager = complainManager;
            hubContext = _hubContext;
            notificationManager = _notificationManager;
        }

        // إضافة شكوى - يجب أن يكون المشتري مخولًا
        [HttpPost("add")]
        [Authorize(Roles = "Buyer")] // يتطلب أن يكون المستخدم Buyer
        public async Task<IActionResult> AddComplain([FromBody] ComplainAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _complainManager.AddComplain(model);

            if (!result)
            {
                return BadRequest("لم يتم العثور على دفع مكتمل أو تم إدخال بيانات غير صحيحة.");
            }
           
            var buyerID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(buyerID))
            {
                if (await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = "Sorry, You Have Complain :( ",
                    IsReaded = false,
                    Title = Enums.NotificationType.complain,
                    UserId = buyerID,
                }))
                {
                    var sellerID = _complainManager.GetAll().Where(c => c.BuyerID == buyerID).Select(c => c.BuyerID).FirstOrDefault();
                    if (sellerID.Any())
                    {
                        Notification lastNotification = notificationManager.GetAll().Where(n=>n.UserId==sellerID).OrderBy(n => n.Id).LastOrDefault();
                        if (lastNotification == null)
                            return BadRequest(new { message = "no last notification found" });

                        await hubContext.Clients.Groups(sellerID).SendAsync("notification", lastNotification.ToViewModel());


                    }
                }
            }

            return Ok("تم إضافة الشكوى بنجاح.");
        }

        // الحصول على الشكاوى - يجب أن يكون المستخدم مخولًا كـ Admin
        [HttpGet("list")]
        [Authorize(Roles = "Admin")] // يتطلب أن يكون المستخدم Admin
        public async Task<IActionResult> GetComplains(int pageNumber = 1, int pageSize = 10, string searchText = "")
        {
            var complains = await _complainManager.GetComplains(pageNumber, pageSize, searchText);
            return Ok(complains);
        }

        [HttpGet("sellers")]
        [Authorize(Roles = "Buyer")] // يتطلب أن يكون المستخدم Buyer
        public async Task<IActionResult> GetSellers()
        {
            var buyerId = GetCurrentBuyerId(); // يجب أن تضيف دالة للحصول على ID المشتري الحالي
            var sellers = await _complainManager.GetSellersByBuyerId(buyerId);
            return Ok(sellers);
        }

        // دالة للحصول على ID المشتري الحالي (تأكد من طريقة استخدامك)
        private int GetCurrentBuyerId()
        {
            var buyerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(buyerIdClaim, out var buyerId) ? buyerId : 0;
        }

    }
}
