using Final;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ModelView
{
    public static class ItemExtesions
    {
        public static Item toItemModel(this AddItemViewModel model)
        {

                List<Image> images = new List<Image>();
                foreach (IFormFile file in model.Images)
                {

                        string filename = DateTime.Now.ToFileTime().ToString() + file.FileName;
                         string path = Path.Combine(
                         Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "Images",
                        "Items",
                        filename
                 );
                FileStream filestream = new FileStream(path, FileMode.Create);
                file.CopyTo(filestream);
                filestream.Close();
                Image img = new Image { Src = Path.Combine("Images", "Items", filename) };
                images.Add(img);
            }
            string fileName = string.Empty;
            if (model.Contract != null)
            {
                fileName = DateTime.Now.ToFileTime().ToString() + model.Contract.FileName;
                string filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "Contracts",

                    fileName
                    );
                FileStream stream = new FileStream(filePath, FileMode.Create);
                model.Contract.CopyTo(stream);
                stream.Close();
                
            }
            var contractfile = Path.Combine("Contracts", fileName);
            return new Item
            {
                Name = model.Title,
                Description = model.Description,
                CategoryID = model.Category,
                StartPrice = model.startPrice,
                EndPrice = model.sellPrice ?? 0,
                AddTime = DateTime.Now,
                Images = images.ToArray(),
                ContractFile = contractfile??"N/A",
                SellerID=model.sellerId
            };

        }

        public static ItemViewModel toItemViewModel(this Item model)
        {
            return new ItemViewModel
            {
                ID = model.ID,
                Title = model.Name,
                Description = model.Description,
                startPrice = model.StartPrice,
                sellPrice = model.EndPrice,
                Images = model.Images.ToArray(),
                Contract = model.ContractFile,
                Category = model.Category.Name,
                status=model.Status

            };
        }
    }
}
