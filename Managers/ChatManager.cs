using FinalApi;
using ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class ChatManager:MainManager<Chat>
    {
        public ChatManager(FinalDbContext _finalDbContext) : base(_finalDbContext)
        {

        }

        public async Task<Chat> GetChatByID(int id)
        {
            return await base.GetOne(id);
        }

        public List<Chat> GetChatsByUserID(string sellerID, string buyerID)
        {
            return base.GetAll().Where(c=>c.BuyerID==buyerID&&c.SellerID==sellerID).ToList();
             
        }

        /*public async Task CreateChat(ChatViewModel chatView)
        {
            await base.Add(chatView.MapToChat());
        }*/

        public async Task DeleteChat(int id)
        {
            var chat =await base.GetOne(id);
            await base.Delete(chat);
        }


    }
}
