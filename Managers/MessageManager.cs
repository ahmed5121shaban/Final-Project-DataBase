using FinalApi;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class MessageManager:MainManager<Message>
    {
        public MessageManager(FinalDbContext _finalDbContext):base(_finalDbContext)
        {
            
        }

        public IEnumerable<Message> GetAllMessages()
        {
            return base.GetAll().ToList(); 
        }
        public IEnumerable<Message> GetAllMessageChat(int _ID)
        {
            return base.GetAll().Where(m=>m.ChatId==_ID).ToList();
        }
        public async Task<Message> GetMessageByID(int _ID)
        {
            var message = await base.GetOne(_ID);
            return message;
        }
       /* public Task AddMessage(MessageViewModel messageView)
        {
            return base.Add(messageView.MapToMessage());
        }
        public Task DeleteMessage(MessageViewModel messageView)
        {
            return base.Delete(messageView.MapToMessage());
        }*/
    }
}
