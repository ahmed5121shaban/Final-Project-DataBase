using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalApi
{
    public class Enums
    {
        public enum Gender
        {
            male,
            female

        }

        public enum PaymentMetod
        {
            paypal,
            stripe

        }

        public enum NotificationType
        {
            events,
            watchlist,
            chat,
            auction,
            complain
        }

        public enum ItemStatus
        {
            pending,
            accepted,
            rejected
        }
        public enum AuctionShippingStatus
        {
            NotStarted, //payment not completed
            Pending, //after payment but admin not confirm it
            OnTheWay, //shipping started
            Delivered, //shipping ended
            Returned //if their an issue
        }

    }
}
