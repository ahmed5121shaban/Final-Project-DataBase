using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public static class NotificationExtension
    {
        public static NotificationViewModel ToViewModel(this Notification _notification)
        {
            return new NotificationViewModel
            {
                Description = _notification.Description,
                Time = _notification.Date,
                Title = _notification.Title,
                IsReaded = false,
                
            };
        }
    }
}
