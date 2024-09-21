using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public static class ChatExetention
    {
        public static Chat MapToChat(this ChatViewModel chatView) 
        { 
            return new Chat { SellerID = chatView.SellerID, BuyerID= chatView.BuyerID,
             ChatMessages = chatView.ChatMessages,IsActive = chatView.IsActive};
        }
        public static ChatViewModel MapToChatViewModel(this Chat chat)
        {
            return new ChatViewModel
            {
                SellerID = chat.SellerID,
                BuyerID = chat.BuyerID,
                ChatMessages = chat.ChatMessages.ToList(),
                IsActive = chat.IsActive
            };
        }
    }
}
