using Final;
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
        [Required]
        public string Message { get; set; }
        [Required,DataType(DataType.DateTime)]
        public DateTime Time { get; set; }
        public int ChatId { get; set; }

    }
}
