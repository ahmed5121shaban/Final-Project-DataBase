using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class ComplainAddViewModel
    {
        public string Reason { get; set; }
        public string SellerID { get; set; }  // البائع الذي سيتم تقديم الشكوى ضده
        public string ? BuyerID { get; set; }  // المشتري الذي يقدم الشكوى
    }
}
