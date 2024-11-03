using FinalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public static class EventExtensions
    {
        public static Event ToModel(this AddEventViewModel _addEventView,ICollection<Item> items) 
        {

            return new Event
            {
                Title = _addEventView.Title,
                Description = _addEventView.Description,
                EndDate = _addEventView.EndDate,
                StartDate = _addEventView.StartDate,
                Type = _addEventView.Type,
                Items = items,
                Image = _addEventView.ImageUrl??"not found",
/*                AdminID = _addEventView.AdminID,
*/            };
        }

        public static EventViewModel ToViewModel(this Event _addEventView)
        {

            return new EventViewModel
            {
                Id = _addEventView.ID,
                Title = _addEventView.Title,
                Description = _addEventView.Description,
                EndDate = _addEventView.EndDate,
                StartDate = _addEventView.StartDate,
                Type = _addEventView.Type,
                //Items = _addEventView.Items.ToList(),
                Image = _addEventView.Image,
/*                AdminID = _addEventView.AdminID,
*/            };
        }

        public static OneEventViewModel ToEventViewModel(this Event _addEventView)
        {
            try
            {
                List<EventItemsListViewModel> eventItems = new List<EventItemsListViewModel>();
                foreach (var item in _addEventView.Items)
                {
                    if (item.Auction != null)
                        eventItems.Add(new EventItemsListViewModel
                        {
                            AuctionId = (int)item.AuctionID,
                            SellerName = item.Seller.User.Name,
                            StartPrice = item.StartPrice,
                            BidCount = item.Auction.Bids.Count,
                            Name = item.Name,
                            Images = item.Images.Select(i => i.Src).ToList(),
                        });
                }

                return new OneEventViewModel
                {
                    Id = _addEventView.ID,
                    EndDate = _addEventView.EndDate,
                    StartDate = _addEventView.StartDate,
                    Image = _addEventView.Image,
                    Name = _addEventView.Title,
                    Auctions = eventItems

                };

            }
            catch (Exception ex) 
            {
                return new OneEventViewModel{};
            }
        }
    }
}
