using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final;
using Models;

namespace ModelView
{
    public static class CategoryExtensions
    {
        public static Category ToModel(this AddCategoryViewModel model)
        {
            string fileName = DateTime.Now.ToFileTime().ToString() + model.Image.FileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);
            FileStream stream = new(path, FileMode.Create);
            model.Image.CopyTo(stream);
            stream.Close();
            var ImagePath = (Path.Combine("images", "products", fileName));
            return new Category
            {
                ID = model.Id == null ? 0 : model.Id.Value,
                Name = model.Name,
                Description = model.Description,
                Image = ImagePath
            };
        }
        //public static AddCategoryViewModel ToAddViewModel(this Category model)
        //{
        //    return new AddCategoryViewModel
        //    {
        //        Id = model.ID,
        //        Name = model.Name,
        //        Description = model.Description,
        //        ImagePath = model.Image

        //    };
        //}
        public static CategoryViewModel ToViewModel(this Category model)
        {
            return new CategoryViewModel
            {
                Id = model.ID,
                Name = model.Name,
                Description = model.Description,
                Image = model.Image
            };

        }


    }
}