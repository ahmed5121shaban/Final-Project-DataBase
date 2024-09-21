using Final;
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
        public string SellerID { get; set; }
        public string BuyerID { get; set; }
        [Required,DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
        public virtual List<Message> ChatMessages { get; set; }
    }
}
