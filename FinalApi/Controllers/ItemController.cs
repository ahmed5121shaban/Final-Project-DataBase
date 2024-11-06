using FinalApi; 
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
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
         ItemManager itemManager;
         AccountManager accountManager;
        private readonly NotificationManager notificationManager;
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly IHubContext<DashboardHub> dashboardHubContext;
        private readonly CategoryManager categoryManager;
        private readonly CloudinaryManager cloudinaryManager;
        private readonly IMemoryCache memoryCache;

        public ItemController(ItemManager _itemManager,AccountManager _accountManager,
            NotificationManager _notificationManager,IHubContext<NotificationsHub> _hubContext,
            IHubContext<DashboardHub> _dashboardHubContext,
            CategoryManager _categoryManager,CloudinaryManager _cloudinaryManager, IMemoryCache _memoryCache)
        {
            this.itemManager = _itemManager;
            this.accountManager = _accountManager;
            notificationManager = _notificationManager;
            hubContext = _hubContext;
            dashboardHubContext = _dashboardHubContext;
            categoryManager = _categoryManager;
            cloudinaryManager = _cloudinaryManager;
            memoryCache = _memoryCache;
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
            User user = await accountManager.GetOne(userId);
            //to check if user is seller
            await accountManager.CheckIfSeller(user);
            //if user is seller, continue adding item,
            //if it was not,add seller role to user and continue adding item too
            List<string> urls = new List<string>();
            foreach (var image in _item.Images)
            {
                urls.Add(await cloudinaryManager.UploadFileAsync(image));
            }
            _item.ImagesUrl = urls;
            if (_item.Contract != null) {
                _item.FileName = await cloudinaryManager.UploadFileAsync(_item.Contract);
            };

            var result = await itemManager.Add(_item.toItemModel());
                if (result == true)
                {
                var cat = await categoryManager.GetOne(_item.Category);
                await dashboardHubContext.Clients.All.SendAsync("category", new { name = cat.Name, value = 1 });
                return Ok();
                }
                else
                {
                    return BadRequest(result);
                }

        }

           
 
        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetItemById(int itemId)
        {    
            var item = itemManager.GetAll().Where(i=>i.ID==itemId).Select(i=>i.toItemViewModel()).FirstOrDefault();
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPatch("Edit")]
        public async Task<IActionResult> Edit([FromForm] EditItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // حذف الصور القديمة من Cloudinary إذا كانت موجودة
                if (model.OldImagesUrl != null && model.OldImagesUrl.Count > 0)
                {
                    foreach (var oldImageUrl in model.OldImagesUrl)
                    {
                        // استخراج publicId من رابط الصورة
                        string publicId = ExtractPublicIdFromUrl(oldImageUrl);
                        await cloudinaryManager.DeleteFileAsync(publicId);  // حذف الصورة القديمة
                    }
                }

                // إعادة تعيين القائمة وتفريغها قبل إضافة الصور الجديدة
                model.ImagesUrl = new List<string>();

                // رفع الصور الجديدة إذا كانت موجودة
                if (model.Images != null && model.Images.Count > 0)
                {
                    foreach (var image in model.Images)
                    {
                        var newImageUrl = await cloudinaryManager.UploadFileAsync(image);  // رفع الصورة الجديدة
                        model.ImagesUrl.Add(newImageUrl);  // إضافة الرابط الجديد فقط
                    }
                }

                // رفع العقد الجديد إذا وُجد
                if (model.Contract != null)
                {
                    model.FileName = await cloudinaryManager.UploadFileAsync(model.Contract);
                }

                // تحديث العنصر
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
                        builder.Append(error.ErrorMessage);
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

        // دالة مساعدة لاستخراج publicId من رابط الصورة
        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath;
            return Path.GetFileNameWithoutExtension(path);
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
      // [Authorize(Roles = "Admin")]
        public async  Task<IActionResult> GetAdminPendingItems()
        {
            var res = itemManager.GetAll()
                .Where(i => i.Status == Enums.ItemStatus.pending)
                .Select(i=>i.toItemViewModel())
                .ToList();
            return Ok(res);
        }

        [Authorize(Roles = "Buyer")]
        [HttpGet("Pending")]
        public IActionResult GetPendingItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = itemManager.GetAll()
                                 .Where(i => i.Status == Enums.ItemStatus.pending && i.SellerID == userId)
                                 .Select(i =>i.toItemViewModel())
                                 .ToList();

            return Ok(res);
        }

        //[Authorize(Roles = "Seller")]
        [HttpGet("Accepted")]
        public async Task<IActionResult> GetAcceptedItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = itemManager.GetAll()
                .Where(i => i.Status == Enums.ItemStatus.accepted && i.SellerID==userId&&i.AuctionID==null)
                .Select(i => i.toItemViewModel())
                .ToList();
            return Ok(res);
        }



        //[Authorize(Roles = "Seller")]
        [HttpGet("Rejected")]
        public async Task<IActionResult> GetRejectedItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = itemManager.GetAll()
                .Where(i => i.Status == Enums.ItemStatus.rejected && i.SellerID == userId)
                .Select(i => i.toItemViewModel())
                .ToList();
            return Ok(res);
        }

        [HttpGet("Accept/{id}")]
        [Authorize]
        public async Task<IActionResult> AcceptItem(int id)
        {
            var item = await itemManager.GetOne(id);
            item.Status = Enums.ItemStatus.accepted;
            var res = await itemManager.Update(item);
            if (res)
            {
                if (await notificationManager.Add(new Notification
                {
                    Title = Enums.NotificationType.auction,
                    UserId = item.SellerID,
                    Date = DateTime.Now,
                    Description = $"admin accepted your item {item.Name} you can now start an auction",
                    IsReaded = false,
                })
                    ) {
                    var lastNotification = notificationManager.GetAll()
                        .Where(n => n.UserId == item.SellerID)
                        .OrderBy(n => n.Id).LastOrDefault();
                    await hubContext.Clients.Group(item.SellerID).SendAsync("notification", lastNotification.ToViewModel());
                }
                // send to dashboard
                await dashboardHubContext.Clients.All.SendAsync("topFiveSeller", new { name = item.Seller.User.Name, count = 1 });
                return Ok(res);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost("Reject/{id}")]
        [Authorize]
        public async Task<IActionResult> RejectItem(int id,[FromBody] string RejectReason)
        {
            var item = await itemManager.GetOne(id);
            item.Status = Enums.ItemStatus.rejected;
            item.PublishFeedback = RejectReason;
            var res = await itemManager.Update(item);
            if (res)
            {
                if (await notificationManager.Add(new Notification
                {
                    Title = Enums.NotificationType.auction,
                    UserId = item.SellerID,
                    Date = DateTime.Now,
                    Description = $"admin rejected your item {item.Name} go to your profile to find out why",
                    IsReaded = false,
                })
                 )
                {
                    var lastNotification = notificationManager.GetAll()
                        .Where(n => n.UserId == item.SellerID)
                        .OrderBy(n => n.Id).LastOrDefault();
                    await hubContext.Clients.Group(item.SellerID).SendAsync("notification", lastNotification.ToViewModel());
                }
                return Ok(res);
            }
            else
            {
                return BadRequest();
            }
        }

       
    }
}
