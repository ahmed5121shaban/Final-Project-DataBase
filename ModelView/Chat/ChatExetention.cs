using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelView
{
    public static class ChatExetention
    {
        /*public static Chat MapToChat(this ChatViewModel chatView) 
        {
            return new Chat
            {
                SellerID = chatView.SenderID,
                BuyerID = chatView.ReceiverID,
                ChatMessages = chatView.ChatMessages,
                IsActive = chatView.IsActive,
                StartDate = chatView.StartDate,
                
            };
        }*/
        public static ChatViewModel MapToChatViewModel(this Chat chat,string _senderID,string _lastMessage,DateTime _messageDate)
        {

            if (_senderID == chat.BuyerID)
                return new ChatViewModel
                {
                    Id = chat.ID,
                    SenderID = chat.BuyerID ,
                    ReceiverID = chat.SellerID,
                    IsActive = chat.IsActive,
                    SenderImage = chat.Buyer.User.Image ?? "",
                    SenderName = chat.Buyer.User.Name,
                    ReceiverName = chat.Seller.User.Name,
                    ReceiverImage = chat.Seller.User.Image ?? "",
                    LastMessage = _lastMessage,
                    MessageDate = _messageDate,
                };
            return new ChatViewModel
            {
                Id = chat.ID,
                SenderID = chat.SellerID,
                ReceiverID = chat.BuyerID,
                IsActive = chat.IsActive,
                SenderImage = chat.Seller.User.Image??"",
                SenderName = chat.Seller.User.Name,
                ReceiverName = chat.Buyer.User.Name,
                ReceiverImage = chat.Buyer.User.Image ?? "",
                LastMessage = _lastMessage,
                MessageDate = _messageDate,

            };
        }
    }
}
