using Final;
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
    }
}
