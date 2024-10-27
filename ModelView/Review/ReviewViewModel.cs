using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class ReviewViewModel
    {
        public string BuyerName {  get; set; }
        public string review { get; set; }

        public string BuyerId {  get; set; }

        public string BuyerImage {  get; set; }
        
        public byte ReviewRange {  get; set; }
        public DateTime Date {  get; set; }

    }
}
