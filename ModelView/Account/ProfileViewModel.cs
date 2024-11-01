using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class ProfileViewModel
    {
        public bool IsSeller { get; set; } = false;
        public string FullName { get; set; }
        public decimal Rate { get; set; }
        public int AuctionsNumber { get; set; }
        public string Address { get; set; }
        public int ReviewsNumber { get; set; }
        public string Image {  get; set; }
        public List<ProfileCatViewModel> FavCategories { get; set; }
        public List<AuctiondetailsViewModel> LatestAuctions { get; set; }
        public List<AuctiondetailsViewModel> WonAuctions { get; set; }
        public List<ReviewDetailViewModel> Reviews { get; set; }
        

    }
}
