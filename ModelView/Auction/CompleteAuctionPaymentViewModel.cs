
using FinalApi;

namespace ModelView
{
    public class CompleteAuctionPaymentViewModel
    {
        public int ID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal Taxes { get; set; }
        public string SellerName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalBids { get; set; }
        public decimal StartPrice { get; set;}
        public decimal EndPrice { get; set; }
        public Enums.PaymentMetod Method { get; set; }
        public string Currency { get; set; }

        public List<string> Images { get; set; }

    }
}
