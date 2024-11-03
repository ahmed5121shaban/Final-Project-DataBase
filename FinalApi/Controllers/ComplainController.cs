using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Managers;
using System.Threading.Tasks;
using ModelView;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using FinalApi;
using Microsoft.EntityFrameworkCore;

namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplainController : ControllerBase
    {
        private readonly ComplainManager _complainManager;
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly NotificationManager notificationManager;
        private readonly FinalDbContext _context;

        public ComplainController(ComplainManager complainManager, IHubContext<NotificationsHub> _hubContext,
            NotificationManager _notificationManager, FinalDbContext context)
        {
            _complainManager = complainManager;
            hubContext = _hubContext;
            notificationManager = _notificationManager;
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not authorized.");
        }

        [Authorize]
        [HttpPost("add")]
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

            try
            {
                var buyerID = GetCurrentUserId();

                if (await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = "Sorry, You Have Complain :( ",
                    IsReaded = false,
                    Title = Enums.NotificationType.complain,
                    UserId = buyerID,
                }))
                {
                    var sellerID = _complainManager.GetAll().Where(c => c.BuyerID == buyerID).Select(c => c.SellerID).FirstOrDefault();
                    if (sellerID != null)
                    {
                        var lastNotification = await notificationManager.GetAll()
                            .Where(n => n.UserId == sellerID)
                            .OrderByDescending(n => n.Id)
                            .FirstOrDefaultAsync();

                        if (lastNotification == null)
                            return BadRequest(new { message = "No last notification found" });

                        await hubContext.Clients.Groups(sellerID).SendAsync("notification", lastNotification.ToViewModel());
                    }
                }

                return Ok("تم إضافة الشكوى بنجاح.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetComplains(int pageNumber = 1, int pageSize = 10, string searchText = "")
        {
            var complains = await _complainManager.GetComplains(pageNumber, pageSize, searchText);
            return Ok(complains);
        }
        [Authorize]
        [HttpGet("sellers")]

        public async Task<IActionResult> GetSellers()
        {
            var buyerId = GetCurrentUserId();
            Console.WriteLine($"Fetching sellers for BuyerID: {buyerId}");

            // استدعاء GetSellersByBuyerId من الـ Manager وإعادة النتيجة كـ ViewModel
            var sellers = await _complainManager.GetSellersByBuyerId(buyerId);

            if (sellers == null || !sellers.Any())
            {
                Console.WriteLine("No sellers found for this buyer.");
                return NotFound("No sellers found for this buyer.");
            }

            return Ok(sellers);
        }

    }
}
