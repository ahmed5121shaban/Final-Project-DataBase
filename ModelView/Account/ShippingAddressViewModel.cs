using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FinalApi.Enums;

namespace ModelView.Account
{
    public class ShipmentDetailsViewModel
    {
        public string ShipmentId { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public ShippingAddressViewModel FromAddress { get; set; }
        public ShippingAddressViewModel ToAddress { get; set; }
        public AuctionShippingStatus Status { get; set; }  // Use enum for status
     
    }

    public class ShippingAddressViewModel
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }


}
