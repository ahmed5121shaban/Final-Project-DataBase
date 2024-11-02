using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;
using Models;

namespace ModelView
{
    public static class CategoryExtensions
    {
        public static Category ToModel(this AddCategoryViewModel model)
        {
            
            return new Category
            {
                //ID = model.Id == null ? 0 : model.Id.Value,
                Name = model.Name,
                Description = model.Description,
                Image = model.ImageUrl??"",
                Icon=model.IconUrl??""
            };
        }
        public static CategoryViewModel ToViewModel(this Category model)
        {
            return new CategoryViewModel
            {
                Id = model.ID,
                Name = model.Name,
                Description = model.Description,
                Image = model.Image,
                Icon=model.Icon,
                items=model.Items.Select(i => i.toItemViewModel()).ToList()
            };

        }

        public static ProfileCatViewModel ToProfileCatViewModel(this Category model)
        {
            return new ProfileCatViewModel()
            {
                id = model.ID,
                Name = model.Name,
                Icon = model.Icon

            };
        }
         public static FavCatViewModel ToFavCatViewModel (this Category model)
        {
            return new FavCatViewModel()
            {
                Name =model.Name,
                Description = model.Description,
                Image= model.Image

            };
        }
    }
}