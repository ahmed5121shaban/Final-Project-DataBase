using FinalApi;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class EventManager : MainManager<Event>
    {
        private readonly ItemManager itemManager;
        private readonly CloudinaryManager cloudinary;
        private readonly CategoryManager categoryManager;

        public EventManager(FinalDbContext _dbContext,ItemManager _itemManager, CloudinaryManager _cloudinary,CategoryManager _categoryManager) : base(_dbContext)
        {
            itemManager = _itemManager;
            cloudinary = _cloudinary;
            categoryManager = _categoryManager;
        }

        public async Task<bool> Add(AddEventViewModel _addEventView)
        {
            List<int> itemsID = new List<int>();
            for(int i = 0; i < _addEventView.itemsID.Length; i++)
            {
                if (_addEventView.itemsID[i] == '-')
                    continue;
                itemsID.Add(int.Parse(_addEventView.itemsID[i].ToString()));
            }
            var items = itemManager.GetAll().ToList();
            ICollection<Item> eventsItems = new List<Item>();
            for (int i = 0; i < itemsID.Count ; i++) {
                for (int j = 0; j < items.Count; j++)
                {
                    if (items[j].ID == itemsID[i])
                        eventsItems.Add(items[j]);
                }
            }
            _addEventView.ImageUrl =await cloudinary.UploadFileAsync(_addEventView.Image);

            return await base.Add(_addEventView.ToModel(eventsItems)); 
        }

        public async Task<List<EventViewModel>> GetAll()
        {
            List<EventViewModel> events = new List<EventViewModel>();
            var res = base.GetAll().ToList();
            foreach (var item in res)
            {
                var categoryName = await categoryManager.GetOne(int.Parse(item.Type));
                events.Add( new EventViewModel
                {
                    Id = item.ID,
/*                    AdminID = item.AdminID,
*/                    Description = item.Description,
                    EndDate = item.EndDate,
                    Image = item.Image,
                    StartDate = item.StartDate,
                    Title = item.Title,
                    Type = categoryName.Name,
                });
            }
            return events;
        }

        public async Task<bool> Delete(int id)
        {
            var ev = await base.Get(id);
            if (ev == null)
                return false;
            await base.Delete(ev);
            return true;
        }
    }
}
