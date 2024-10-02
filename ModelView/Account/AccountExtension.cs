using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView.Account
{
    public static class AccountExtension
    {
        public static User ToModel(this RegisterViewModel model)
        {
            return new User
            {
                UserName = $"{model.FirstName}{model.LastName}",
                Name = $"{model.FirstName} {model.LastName}",
                Email = model.Email,
            };
        }

        //public static User ToModel(this LoginViewModel model) 
        //{
        //    return new User 
        //    {
        //       Email = model.Email,
               
        //    };
        //}
    }
}
