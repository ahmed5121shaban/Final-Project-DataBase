using Final;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;

namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api[controller]")]
    public class ItemController : ControllerBase
    {
        private ItemManager itemManager;
        private AccountManager accountManager;
        public ItemController(ItemManager _itemManager, AccountManager _accountManager)
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
                _item.SellerId = userId;
            }
            //get user by id 
            User user = await accountManager.GetOne(userId);
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


        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var items = itemManager.GetAll();
            return Ok(items);
        }

        [HttpGet("GetOne/{itemId}")]
        public async Task<IActionResult> GetItemById(int itemId)
        {
            var item = itemManager.GetAll().Where(i => i.ID == itemId).Select(i => i.toItemViewModel()).FirstOrDefault();
            return Ok(item);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await itemManager.GetOne(id);
            var result = await itemManager.Delete(item);
            if (result)
                return Ok();
            else
                return BadRequest("failed to delete");
        }



    }
}
