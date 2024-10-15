using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class AddEventViewModel
    {
        [Required, DataType(DataType.Text)]
        public string Title { get; set; }
        [Required,DataType(DataType.Text)]
        public string Description { get; set; }
        public string Type { get; set; }
        [Required, DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [Required,DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        [Required]
        public string itemsID { get; set; }
        public string? ImageUrl { get; set; }
        public string? AdminID { get; set; }
    }
}
