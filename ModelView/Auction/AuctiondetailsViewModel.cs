using FinalApi;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FinalApi.Enums;

namespace ModelView
{
    public class AuctiondetailsViewModel
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ItemID { get; set; }
        public bool IsEnded {  get; set; }
        public bool Completed { get; set; }
        public decimal currentPrice { get; set; }
        public AuctionItemViewModel Item { get; set; }
        public List<AutionBidViewModel> Bids { get; set; }
    }
    public class AutionBidViewModel
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
        public string BuyerID { get; set; }
        public string BuyerName { get; set; }
    }
    public class AuctionItemViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public string Description { get; set; }
        public ItemStatus status { get; set; }
        public DateTime AddTime { get; set; }
        public decimal StartPrice { get; set; }
        public List<string> Images { get; set; }
    }
}
