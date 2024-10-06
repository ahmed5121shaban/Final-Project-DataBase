using Final;
using Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;
using System.Text;

namespace FinalApi.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class BidController : ControllerBase
    {
        
        private BidManager manager;
        public BidController(BidManager _manager )
        {
            manager= _manager;
            
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(AddBidViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.BuyerID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                model.Time = DateTime.Now;
                var res =await manager.Add(model.ToModel());
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
        [Route("GethighestBid/{AuctionId}")]
        public IActionResult GethighestBid(int AuctionId)
        {
            var res = manager.GetHighest(AuctionId);
            if (res != null)
            {
                return new JsonResult(new ApiResultModel<Bid>()
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
