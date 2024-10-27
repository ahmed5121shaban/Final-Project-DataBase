using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class NotificationManager : MainManager<Notification>
    {
        public NotificationManager(FinalDbContext _dbContext) : base(_dbContext)
        {
        }

        public async Task<bool> Add(Notification _notification)
        {
            try
            {
                await base.Add(_notification);
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }

      //public async Task<bool> SendAddedAuctionAlert(Notification _notification){}

    }
}
