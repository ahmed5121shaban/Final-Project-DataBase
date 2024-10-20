using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView.Complain
{
    public class ComplainDisplayViewModel
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string SellerName { get; set; }  // معلومات البائع للعرض
        public string BuyerName { get; set; }  // معلومات المشتري للعرض
    }
}
