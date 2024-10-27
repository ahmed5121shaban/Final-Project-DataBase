using FinalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class MessageViewModel
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public int ChatId { get; set; }
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public bool Sender { get; set; }

    }
}
