using Final; 
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
namespace FinalApi.Controllers
{

    [ApiController]

    [Route("api/[controller]")]
    public class ItemController:ControllerBase
    {
        private ItemManager itemManager;
        private AccountManager accountManager;
        public ItemController(ItemManager _itemManager,AccountManager _accountManager)
        {
            this.itemManager = _itemManager;
            this.accountManager = _accountManager;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddItem([FromForm] AddItemViewModel _item)
        {
            //get userid from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                //set userid to additemviewmodel
                _item.sellerId = userId;
            }
            //get user by id 
            User user =await accountManager.GetOne(userId);
            //to check if user is seller
            await accountManager.CheckIfSeller(user);
            //if user is seller, continue adding item,
            //if it was not,add seller role to user and continue adding item too
            var result = await itemManager.Add(_item.toItemModel());
                if (result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            
        }

           


        
        //to display only items of this user by its id
        [Authorize]
        [HttpGet]
        //public async Task<IActionResult> GetUserItems()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var items = itemManager.GetAll().Where(i=>i.SellerID==userId).Select(i=>i.toItemViewModel());
        //    return Ok(items);
        //}

        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetItemById(int itemId)
        {
            var item = itemManager.GetAll().Where(i=>i.ID==itemId).Select(i=>i.toItemViewModel()).FirstOrDefault();
            return Ok(item);
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit(AddItemViewModel model)
        {
            if (ModelState.IsValid)
            {
               var res = await itemManager.Update(model.toItemModel());
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
                    return new JsonResult(new ApiResultModel<string>()
                    {
                        result = "",
                        StatusCode = 200,
                        success = true,
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
       

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await itemManager.Get(id);
            var result = await itemManager.Delete(item);
            if (result)
                return Ok();
            else
                return BadRequest("failed to delete");
        }
        [HttpGet("Unreviewed")]
       [Authorize(Roles = "Admin")]
        public  IActionResult GetAdminPendingItems()
        {

            var res = itemManager.GetAll().Where(i => i.Status == Enums.ItemStatus.pending);
            return Ok(res);
        }
        [HttpGet("Pending")]
       [Authorize(Roles = "Seller")]
        public IActionResult GetPendingItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = itemManager.GetAll().Where(i => i.Status == Enums.ItemStatus.pending && i.SellerID==userId);
            return Ok(res);
        }
        [HttpGet("Accepted")]
         [Authorize(Roles = "Seller")]
        public IActionResult GetAcceptedItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = itemManager.GetAll().Where(i => i.Status == Enums.ItemStatus.accepted && i.SellerID==userId);
            return Ok(res);
        }
        [HttpGet("Rejected")]
         [Authorize(Roles = "Seller")]
        public IActionResult GetRejectedItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = itemManager.GetAll().Where(i => i.Status == Enums.ItemStatus.rejected && i.SellerID == userId);
            return Ok(res);
        }
        [HttpPut("Accept/{id}")]
       // [Authorize(Roles ="Seller")]
        public async Task<IActionResult> AcceptItem(int id)
        {
            var item = await itemManager.GetOne(id);
            item.Status = Enums.ItemStatus.accepted;
            var res = await itemManager.Update(item);
            if (res)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut("Reject/{id}")]
        // [Authorize(Roles ="Admin")]
        public async Task<IActionResult> RejectItem(int id,string RejectReason)
        {
            var item = await itemManager.GetOne(id);
            item.Status = Enums.ItemStatus.rejected;
            item.PublishFeedback = RejectReason;
            var res = await itemManager.Update(item);
            if (res)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
