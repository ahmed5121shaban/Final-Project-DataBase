using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Models;
using Models.Models;
using ModelView;

namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavCategoryController : ControllerBase
    {
        private readonly FavCategoryManager favmanager ;
        private readonly BuyerManager buyerManager ;
        private readonly CategoryManager categorymanager;
        public FavCategoryController(FavCategoryManager _favmanager,
            BuyerManager _buyermanager,
            CategoryManager _categorymanager) 
        {
            favmanager = _favmanager;
            buyerManager= _buyermanager;
            categorymanager= _categorymanager;
        
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCategoryToFavorite([FromBody] int categoryId)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var buyer = buyerManager.GetAll().Where(i => i.UserID == userId).FirstOrDefault();
            var category = await categorymanager.GetOne(categoryId);
            var Fav = favmanager.GetAll().Where(i => i.BuyerID == userId && i.CategoryID == categoryId).FirstOrDefault();

            if (Fav != null)
            {
                var res = await favmanager.Delete(Fav);
                if (res)
                {
                    return new JsonResult(new ApiResultModel<string>()
                    {
                        result = "removed",
                        success = true,
                        StatusCode = 200,
                        Message = "done successfully"

                    });

                }
                else
                {
                    return new JsonResult(new ApiResultModel<string>()
                    {
                        result = "",
                        success = true,
                        StatusCode = 200,
                        Message = "something wrong happend"

                    });

                }



            }
            else
            {
               var res = await favmanager.Add(new FavCategories()
                {
                    BuyerID = userId,
                    Buyer = buyer,
                    CategoryID = categoryId,
                    Category = category

                });

                if (res)
                {
                    return new JsonResult(new ApiResultModel<string>()
                    {
                        result = "added",
                        success = true,
                        StatusCode = 200,
                        Message = "done successfully"

                    });

                }
                else
                {
                    return new JsonResult(new ApiResultModel<string>()
                    {
                        result = "",
                        success = true,
                        StatusCode = 200,
                        Message = "something wrong happend"

                    });

                }
                


            }
            
        }
        [HttpGet]
        [Authorize]
         public IActionResult getFavCategoryIds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var idList = favmanager.GetAll().Where(i => i.BuyerID == userId).Select(i => i.CategoryID).ToList();
            return Ok(idList);

        }

        [HttpGet("AllFav")]
        [Authorize]
        public IActionResult getAllfavCategories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var categoryList = favmanager.GetAll().Where(i => i.BuyerID == userId).Select(i=>i.Category.ToFavCatViewModel()).ToList();
            return Ok(categoryList);

        }

       

    }
}
