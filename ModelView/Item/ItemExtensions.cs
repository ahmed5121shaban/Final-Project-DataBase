using FinalApi;
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
                
            foreach (string imageUrl in model.ImagesUrl)
            {      
                Image img = new Image { Src = imageUrl };
                images.Add(img);
            }
            
            return new Item
            {
                Name = model.Title,
                Description = model.Description,
                CategoryID = model.Category,
                StartPrice = model.startPrice,
                EndPrice = model.sellPrice ?? 0,
                AddTime = DateTime.Now,
                Images = images.ToArray(),
                ContractFile = model.FileName??"N/A",
                SellerID=model.sellerId
            };

        }
        public static Item toItemModel(this EditItemViewModel model)
        {

            List<Image> images = new List<Image>();
            if(model.ImagesUrl != null)
                foreach (string imageUrl in model.ImagesUrl)
                {
                    Image img = new Image { Src = imageUrl };
                    images.Add(img);
                }

            return new Item
            {
                ID=model.itemId,
                Name = model.Title,
                Description = model.Description,
                CategoryID = (int)model.Category,
                StartPrice = (decimal)model.startPrice,
                EndPrice = model.sellPrice ?? 0,
                AddTime = DateTime.Now,
                Images = images.ToArray(),
                ContractFile = model.FileName ?? "N/A",
                SellerID = model.sellerId
            };

        }

        public static ItemViewModel toItemViewModel(this Item model)
        {
            return new ItemViewModel
            {
                id = model.ID,
                name = model.Name,
                startPrice = model.StartPrice,
                sellPrice = model.EndPrice,
                status = model.Status,
                category = model.Category.Name,
                sellerName = model.Seller.User.Name,
                categoryId = model.CategoryID,
                description = model.Description,
                contract=model.ContractFile,
                publishFeedback=model.PublishFeedback,
                images = model.Images.Select(img=>img.toImageViewModel()).ToArray(),

            };
        }
    }
}
