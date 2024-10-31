using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace ModelView
{
    public class AddCategoryViewModel
    {
        //public int? Id { get; set; }
        [Required]
        [MaxLength(15)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = " Choose Category Image")]
        public IFormFile Image { get; set; }
        public IFormFile Icon { get; set; }

        public string? ImageUrl { get; set; }
        public string? IconUrl { get; set; }


    }
}
