using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class EditItemViewModel
    {
        public int itemId { get; set; }
        public string? Title { get; set; }
        public int? Category { get; set; }
        public string? Description { get; set; }

        public string? sellerId { get; set; }

        public decimal? sellPrice { get; set; }
        public decimal? startPrice { get; set; }
        public IFormFile? Contract { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<string>? ImagesUrl { get; set; }
        public List<string>? OldImagesUrl { get; set; }

        public string? FileName { get; set; }
    }
}
