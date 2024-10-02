using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class LoginViewModel
    {
        [Required, StringLength(50, MinimumLength = 8)]
        public string Email { get; set; }

        [Required, StringLength(50, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RemeberMe { get; set; } = false;

        public string ReturnUrl { get; set; } = "/";
    }
}
