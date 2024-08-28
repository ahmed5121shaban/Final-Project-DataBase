using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zaeid.models
{
    public class Image
    {
        public int ID { get; set; }
        public string Src {  get; set; }

        public int userID { get; set; }
        public virtual User User { get; set; }


    }
}
