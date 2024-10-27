using FinalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class ChatViewModel
    {
        public int Id { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        [Required,DataType(DataType.DateTime)]
        public DateTime MessageDate { get; set; }
        public bool IsActive { get; set; }
        public string SenderImage { get; set; }
        public string ReceiverImage { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string LastMessage { get; set; }
    }
}
