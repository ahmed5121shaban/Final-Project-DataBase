using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelView
{
    public static class MessageExetention
    {
        public static Message MapToMessage(this MessageViewModel messageView) 
        { 
            return new Message { Text=messageView.Message, ChatId=messageView.ChatId, Time=messageView.Time}; 
        }

        public static MessageViewModel MapToMessageViewModel(this Message messageView)
        {
            return new MessageViewModel { Message = messageView.Text, ChatId = messageView.ChatId,
                Time = messageView.Time };
        }
    }
}
