using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView.Account
{
    public class ResetPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [Required, Display(Name = "New Password"),
        DataType(DataType.Password), StringLength(10, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
