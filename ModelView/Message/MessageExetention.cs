using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelView
{
    public static class MessageExetention
    {
        public static Message MapToMessage(this AddMessageViewModel messageView) 
        {
            return new Message { Text = messageView.Message, ChatId = messageView.ChatId, Time = messageView.Time };
        }

        public static MessageViewModel MapToMessageViewModel(this Message message,string _userID)
        {
            if (_userID == message.UserID)
                return new MessageViewModel
                {
                    ChatId = message.ChatId,
                    Sender = true,
                    Message = message.Text,
                    Time = message.Time,
                    UserImage = message.User.Image??"",
                    UserName = message.User.Name,
                };
            return new MessageViewModel
            {
                ChatId = message.ChatId,
                Sender = false,
                Message = message.Text,
                Time = message.Time,
                UserImage = message.User.Image ?? "",
                UserName = message.User.Name,
            };
            /*
            if (_userID == message.Chat.BuyerID)
                return new MessageViewModel
                {
                    Message = message.Text,
                    ChatId = message.ChatId,
                    Time = message.Time,
                    ReceiverImage = message.Chat.Seller.User.Image ?? "",
                    SenderImage = message.Chat.Buyer.User.Image ?? "",
                    ReceiverName = message.Chat.Seller.User.Name,
                    SenderName = message.Chat.Buyer.User.Name,
                    Sender = "buyer"
                };
            return new MessageViewModel
            {
                Message = message.Text,
                ChatId = message.ChatId,
                Time = message.Time,
                ReceiverImage = message.Chat.Buyer.User.Image ?? "",
                SenderImage = message.Chat.Seller.User.Image ?? "",
                ReceiverName = message.Chat.Buyer.User.Name,
                SenderName = message.Chat.Seller.User.Name,
                Sender = "seller"
            };*/
        }
    }
}
