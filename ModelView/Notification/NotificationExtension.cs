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
            string subjectName = string.Empty;
            if (_notification.User == null)
                subjectName = "no user name found";
            else subjectName = _notification.User.Name;
            
            return new NotificationViewModel
            {
                Description = _notification.Description,
                Time = _notification.Date,
                Title = _notification.Title,
                SubjectName = subjectName,
                IsReaded = false,
                
            };
        }
    }
}
