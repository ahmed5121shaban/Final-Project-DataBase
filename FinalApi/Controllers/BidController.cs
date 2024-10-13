using Final;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;
using System.Text;

namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        
        private BidManager manager;
        private readonly UserManager<User> userManager;
        private readonly PaymentManager paymentManager;

        public BidController(BidManager _manager,UserManager<User> _userManager,PaymentManager _paymentManager)
        {
            manager= _manager;
            userManager = _userManager;
            paymentManager = _paymentManager;
        }


        [HttpGet("auction-payment-method{_auctionID:int}/{_paymentID:int}/{_metod}")]
        public IActionResult AuctionPaymentMethod(int _auctionID,int _paymentID, Enums.PaymentMetod  _metod)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userManager.Users.FirstOrDefault( u => u.Id == Id );
            if (user == null) 
            {
                return NotFound();
            }
            
            paymentManager.Add(new PaymentViewModel { AuctionID = _auctionID, BuyerId = Id,  Method = _metod });

            return Ok();
        }

        [HttpPost("first-auction-payment")]
        public IActionResult MinceFirstAuctionPayment(PaymentViewModel _paymentView)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(new { message = "this data is not completed"});
            }
            if (_paymentView.Method == Enums.PaymentMetod.paypal)
            {
                if (paymentManager.AddPayPalPayment(_paymentView)) return Ok();
                return BadRequest(new { message = "the paypal payment is not completed" });

            }
            if (paymentManager.AddStripePayment(_paymentView)) return Ok();
            return BadRequest(new { message = "the stripe payment is not completed" });

        }


        [HttpGet("user-have-payment")]
        public IActionResult GetPaymentMethodsCount() 
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
                return BadRequest(new {message="the user not found"});
            var user = userManager.Users.FirstOrDefault(u => u.Id == userID);
            if (string.IsNullOrEmpty(user.PaypalEmail) && string.IsNullOrEmpty(user.StripeEmail))
                return Ok(new {count = 0});
            else if (string.IsNullOrEmpty(user.PaypalEmail) && !string.IsNullOrEmpty(user.StripeEmail))
                return Ok(new { count = 1, payment = "stripe" });
            else if (!string.IsNullOrEmpty(user.PaypalEmail) && string.IsNullOrEmpty(user.StripeEmail))
                return Ok(new { count = 1, payment = "paypal" });
            return Ok(new { count = 2 });
        }

        [Authorize]
        [HttpPost("Add")]
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
