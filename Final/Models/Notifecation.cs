using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Notifecation
    {
        public int ID {  get; set; }
        public string Massege { get; set; }

        public DateTime DateTime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }


    }
}
