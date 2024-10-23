using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView.Profile
{
    public class BuyerProfileViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public int WonAuctions { get; set; }
        public int LostAuctions { get; set; }
        public decimal ProfileCompletion { get; set; }
    }

}
