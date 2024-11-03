using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatManager chatManager;
        private readonly PaymentManager paymentManager;
        private readonly AuctionManager auctionManager;
        private readonly MessageManager messageManager;

        public ChatController(ChatManager _chatManager,PaymentManager _paymentManager,
            AuctionManager _auctionManager,MessageManager _messageManager)
        {
            chatManager = _chatManager;
            paymentManager = _paymentManager;
            auctionManager = _auctionManager;
            messageManager = _messageManager;
        }

        [HttpGet("Chats")]
        public IActionResult GetChats()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "user not found" });


            List<ChatViewModel> chats = new List<ChatViewModel>();
            foreach (var item in chatManager.GetAll().Where(c=>c.BuyerID == userId||c.SellerID==userId).ToList())
            {
                var message = messageManager.GetAll()
                    .Where(m => m.ChatId == item.ID)
                    .Select(m => new { m.Text, m.Time })
                    .OrderBy(m=>m.Time).LastOrDefault();
                chats.Add(item.MapToChatViewModel(userId, message?.Text??"",message?.Time??DateTime.Now));
            }
            if (!chats.Any())
                return BadRequest(new { message = "no chats found" });

            return new JsonResult(new ApiResultModel<List<ChatViewModel>>
            {
                Message = "the result completed",
                result = chats,
                success = true,
                StatusCode = 200
            });
        }
    }


}
