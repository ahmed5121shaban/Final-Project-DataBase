using FinalApi;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;
using ModelView;
using System.ComponentModel;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private CategoryManager manager;
        private readonly CloudinaryManager cloudinaryManager;
        private readonly IMemoryCache memoryCache;

        public CategoryController(CategoryManager _manager, CloudinaryManager _cloudinaryManager,IMemoryCache _memoryCache)
        {
            manager = _manager;
            cloudinaryManager = _cloudinaryManager;
            memoryCache = _memoryCache;
        }
        [Authorize]
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromForm] AddCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                if(model.Icon!=null)
                    model.IconUrl =await cloudinaryManager.UploadFileAsync(model.Icon);
                if (model.Image != null)
                    model.ImageUrl = await cloudinaryManager.UploadFileAsync(model.Image);
                var res = await manager.Add(model.ToModel());
                if (res)
                {
                    return new JsonResult(new ApiResultModel<bool>()
                    {
                        result = res,
                        StatusCode = 200,
                        success = true,
                        Message = "done successfully"

                    });
                }
                else
                {
                    return new JsonResult(new ApiResultModel<bool>()
                    {
                        result = res,
                        StatusCode = 400,
                        success = false,
                        Message = "An Error Has Occured"

                    });

                }
            }
            else
            {
                var builder = new StringBuilder();
                foreach (var item in ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        builder.Append(builder.ToString());
                    }
                }
                return new JsonResult(new ApiResultModel<string>()
                {
                    result = builder.ToString(),
                    StatusCode = 400,
                    success = false,
                    Message = "Not Valid Model"
                });
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            if (memoryCache.TryGetValue("getallcategories", out var resultcache))
                return new JsonResult(new ApiResultModel<object>()
                {
                    result = resultcache,
                    StatusCode = 200,
                    success = true,
                    Message = "done successfully"

                });
            var res = manager.GetAll().Select(c => new
            {
                id = c.ID,
                name = c.Name,
                image = c.Image,
                icon=c.Icon,
                description=c.Description,
                items = c.Items.Select(i => i.toItemViewModel()).ToArray()
            }).ToList();
            if (res != null)
            {
                memoryCache.Set("getallcategories", res);
                return new JsonResult(new ApiResultModel<object>()
                {
                    result = res,
                    StatusCode = 200,
                    success = true,
                    Message = "done successfully"

                });
            }
            else
            {
                return new JsonResult(new ApiResultModel<string>()
                {
                    result = "",
                    StatusCode = 200,
                    success = true,
                    Message = "An Error Has Occured"

                });

            }
        }

        [HttpDelete]
        [Route("Delete/{Id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await manager.GetOne(id);
            var res = await manager.Delete(category);
            if (res)
            {
                return new JsonResult(new ApiResultModel<bool>()
                {
                    result = res,
                    StatusCode = 200,
                    success = true,
                    Message = "done successfully"

                });

            }
            else
            {
                return new JsonResult(new ApiResultModel<bool>()
                {
                    result = res,
                    StatusCode = 400,
                    success = false,
                    Message = "An Error Has Occured"

                });
            }

        }
        [HttpGet("Filter/{searchtxt}")]
        public IActionResult Pagination(string searchtxt = "")
        {
            var res = manager.Get(searchtxt);
            if (res != null)
            {
                return new JsonResult(new ApiResultModel<Pagination<List<CategoryViewModel>>>()
                {
                    result = res,
                    StatusCode = 200,
                    success = true,
                    Message = "done successfully"

                });
            }
            else
            {
                return new JsonResult(new ApiResultModel<string>()
                {
                    result = "",
                    StatusCode = 400,
                    success = false,
                    Message = "An Error Has Occured"

                });
            }

        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AddCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await manager.GetOne(1);
                category.Name = model.Name;
                category.Description = model.Description;

                if (model.Icon != null)
                    model.IconUrl = await cloudinaryManager.UploadFileAsync(model.Icon);
                category.Icon = model.IconUrl;
                if (model.Image != null)
                    model.ImageUrl = await cloudinaryManager.UploadFileAsync(model.Image);
                category.Image = model.ImageUrl;


                var res = await manager.Update(category);
                if (res)
                {
                    return new JsonResult(new ApiResultModel<bool> { result = res, StatusCode = 200, success = true, Message = "done successfully" });
                }
                else
                {
                    return new JsonResult(new ApiResultModel<string> { result = "", StatusCode = 200, success = false, Message = "An Error Has Occured" });
                }
            }

            return new JsonResult(new ApiResultModel<string> { result = "Not Valid Model", StatusCode = 400, success = false });
        }



        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            if(memoryCache.TryGetValue($"GetCategoryById{id}", out var result))
                return Ok(result);
            try
            {
                var category =await manager.GetOne(id);
                memoryCache.Set($"GetCategoryById{id}",category.ToViewModel());
                return Ok(category.ToViewModel());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the category.", Error = ex.Message });
            }
        }
    }
}