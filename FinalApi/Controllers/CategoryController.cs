using FinalApi;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using ModelView;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private CategoryManager manager;

        public CategoryController(CategoryManager _manager)
        {
            manager = _manager;
        }
        [Authorize]
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromForm] AddCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {

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
            var res = manager.GetAll().Select(c => new
            {
                id = c.ID,
                name = c.Name,
                image = c.Image,
                icon=c.Icon,
                items = c.Items.Select(i => i.toItemViewModel()).ToArray()
            }).ToList();
            if (res != null)
            {
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
        [HttpGet]
        [Route("Filter")]
        public IActionResult Pagination([FromQuery] string searchText, string calumnName, bool isAscending, int pageSize, int PageNumber)
        {
            var res = manager.Get(searchText, calumnName, isAscending, pageSize, PageNumber);
            if (res != null)
            {
                return new JsonResult(new ApiResultModel<Pagination<List<Category>>>()
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


    }
}