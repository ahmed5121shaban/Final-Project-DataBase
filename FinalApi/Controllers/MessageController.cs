using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageManager messageManager;

        public MessageController(MessageManager _messageManager)
        {
            messageManager = _messageManager;
        }

        [HttpGet("messages/{_chatID:int}")]
        public async Task<IActionResult> GetMessages(int _chatID)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "user not found" });

            List<MessageViewModel> messageViewModels = new List<MessageViewModel>();
            foreach (var item in messageManager.GetAll().Where(m => m.ChatId == _chatID).ToList())
            {
                messageViewModels.Add(item.MapToMessageViewModel(userId));
            }
            if (messageViewModels.Any())
                return new JsonResult(new ApiResultModel<List<MessageViewModel>>
                {
                    Message = "fetching the data is completed",
                    success = true,
                    result = messageViewModels,
                    StatusCode = 200
                });
            return new JsonResult(new ApiResultModel<string>
            {
                Message = "no message found",
                success = true,
                result = null,
                StatusCode = 200
            });
        }
    }
}
