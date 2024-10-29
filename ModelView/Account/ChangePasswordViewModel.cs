using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace ModelView
{
    public class ChangePasswordViewModel
    {
        public string UserID { get; set; } = "";


        [Required, Display(Name = "Current Password"),
         DataType(DataType.Password), StringLength(10, MinimumLength = 8)]
        public string CurrentPassword { get; set; }


        [Required, Display(Name = "New Password"),
         DataType(DataType.Password), StringLength(10, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
