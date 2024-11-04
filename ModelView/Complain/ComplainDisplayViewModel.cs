using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class ComplainDisplayViewModel
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string SellerName { get; set; }  // معلومات البائع للعرض
        public string BuyerName { get; set; }
        public string selleremail { get; set; }
        public string buyeremail { get; set; }

        // معلومات المشتري للعرض
    }
}
