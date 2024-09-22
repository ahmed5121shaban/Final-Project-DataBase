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
                Name = model.FullName,
                Email = model.Email,
            };
        }
    }
}
