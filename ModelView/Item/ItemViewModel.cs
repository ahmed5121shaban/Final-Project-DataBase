using Final;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Final.Enums;

namespace ModelView
{
    public class ItemViewModel
    {
        public int  ID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public ItemStatus status { get; set; }
        public decimal? sellPrice { get; set; }
        public decimal startPrice { get; set; }

        public string? Contract { get; set; }
        public Image[] Images { get; set; }
    }
}
