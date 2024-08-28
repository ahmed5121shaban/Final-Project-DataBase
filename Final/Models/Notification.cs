using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finalproject.Models
{


    public enum NotificationType
    {
        watchlist,
        auctionstarted

    }
    public class Notification
    {
        public int Id { get; set; }
        public NotificationType Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsReaded { get; set; }

    }
}
