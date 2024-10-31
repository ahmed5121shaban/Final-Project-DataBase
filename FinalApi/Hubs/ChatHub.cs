using Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ModelView;
using System.Security.Claims;

namespace FinalApi
{
    public class ChatHub:Hub
    {
        private readonly ChatManager chatManager;
        private readonly MessageManager messageManager;
        private readonly UserManager<User> userManager;

        public ChatHub(ChatManager _chatManager,MessageManager _messageManager,UserManager<User> _userManager)
        {
            chatManager = _chatManager;
            messageManager = _messageManager;
            userManager = _userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var chatID = Context.GetHttpContext().Request.Query["chatID"];
            if (!string.IsNullOrEmpty(chatID))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatID);
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string chatID, string message)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var chatMessage = new Message
            {
                ChatId = int.Parse(chatID),
                Text = message,
                Time = DateTime.UtcNow,
                UserID = userId,
            };

            if (!await messageManager.Add(chatMessage))
                return;

            var lastMessage = messageManager.GetAll().Where(m=>m.ChatId== int.Parse(chatID)).OrderBy(m=>m.Id).LastOrDefault();
            lastMessage.Chat =await chatManager.GetOne(int.Parse(chatID));
            lastMessage.User = await userManager.FindByIdAsync(userId);
            await Clients.Group(chatID).SendAsync("getMessages", lastMessage.MapToMessageViewModel(userId));
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var chatID = Context.GetHttpContext().Request.Query["chatID"];
            if (!string.IsNullOrEmpty(chatID))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatID);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
