using FinalApi;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FinalApi.Enums;

namespace ModelView
{
    public class ItemViewModel
    {
        public int  id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public int categoryId { get; set; }
        public string sellerName { get; set; }
        public string description { get; set; }
        public ItemStatus status { get; set; }
        public decimal? sellPrice { get; set; }
        public decimal startPrice { get; set; }
        public string? publishFeedback { get; set; }
        public string? contract { get; set; }
        public ImageViewModel[] images { get; set; }
    }
}
