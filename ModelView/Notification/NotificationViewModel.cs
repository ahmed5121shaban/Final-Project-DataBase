using FinalApi;
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
        //public string TitleText { get; set; }
        public string Description { get; set; }
        public string SubjectName { get; set; }
        public bool IsReaded { get; set; }

    }
}
