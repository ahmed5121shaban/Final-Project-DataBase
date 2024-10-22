using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class NotificationViewModel
    {
        public DateTime Time { get; set; }
        public Enums.NotificationType Title { get; set; }
        public string Description { get; set; }
        public string SenderName { get; set; }

    }
}
