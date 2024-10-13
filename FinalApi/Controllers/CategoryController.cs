﻿using Final;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using ModelView;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController:ControllerBase
    {
        private CategoryManager manager;

        public CategoryController(CategoryManager _manager)
        {
            manager = _manager;
        }
        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add(AddCategoryViewModel model) {

            if (ModelState.IsValid)
            {
                string fileName = DateTime.Now.ToFileTime().ToString() + model.Image.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);
                FileStream stream = new(path, FileMode.Create);
                model.Image.CopyTo(stream);
                stream.Close();
                model.ImagePath = (Path.Combine("images", "products", fileName));
                var res = await manager.Add(model.ToModel());
                if (res)
                {
                    return new JsonResult(new ApiResultModel<bool>()
                    {
                        result = res,
                        StatusCode = 200,
                        success = true,
                        Message="done successfully"

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
                foreach(var item in ModelState.Values)
                {
                    foreach(var error in item.Errors)
                    {
                        builder.Append (builder.ToString());
                    }
                }
                return new JsonResult(new ApiResultModel<string>()
                {
                    result= builder.ToString(),
                    StatusCode=400,
                    success =false,
                    Message ="Not Valid Model"
                });
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            var res = manager.GetAll().ToList();
            if(res!= null)
            {
                return new JsonResult(new ApiResultModel<List<Category>>()
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
        public async Task<IActionResult> Delete(int id) {
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
        public IActionResult Pagination([FromQuery] string searchText,string calumnName,bool isAscending,int pageSize ,int PageNumber)
        {
            var res = manager.Get(searchText,calumnName,isAscending,pageSize,PageNumber);
            if(res != null)
            {
                return new JsonResult(new ApiResultModel<Pagination<List<Category>>>()
                {
                    result =res,
                    StatusCode = 200,
                    success=true,
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