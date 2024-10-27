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
            //for icon
            string iconfileName = DateTime.Now.ToFileTime().ToString() + model.Icon.FileName;
            string iconpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Categories", iconfileName);
            FileStream iconstream = new(iconpath, FileMode.Create);
            model.Icon.CopyTo(iconstream);
            iconstream.Close();
            var IconPath = (Path.Combine("Images", "Categories", iconfileName));



            //for image
            string fileName = DateTime.Now.ToFileTime().ToString() + model.Image.FileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Categories", fileName);
            FileStream stream = new(path, FileMode.Create);
            model.Image.CopyTo(stream);
            stream.Close();
            var ImagePath = (Path.Combine("Images", "Categories", fileName));
            return new Category
            {
                //ID = model.Id == null ? 0 : model.Id.Value,
                Name = model.Name,
                Description = model.Description,
                Image = ImagePath,
                Icon=IconPath
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