using Final;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using Managers;
using Stripe;
using System.Security.Claims;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;
        private readonly PaymentManager paymentManager;

        public PaymentController(IConfiguration _configuration,UserManager<User> _userManager, PaymentManager _paymentManager)
        {
            configuration = _configuration;
            userManager = _userManager;
            paymentManager = _paymentManager;
        }

        [HttpPost("add-payment-email")]
        public IActionResult AddPaymentEmail([FromBody] PaymentViewModel paymentView)
        {
            
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if(paymentView.Method== Enums.PaymentMetod.paypal)
                user.PaypalEmail = paymentView.PaypalEmail;
            if (paymentView.Method == Enums.PaymentMetod.stripe)
                user.StripeEmail = paymentView.StripeEmail;


            userManager.UpdateAsync(user);

            return Ok(new { message = "PayPal payment method added." });
        }

        [HttpPost("add-paypal-payment")]
        public IActionResult AddPayPalPayment([FromBody] PaymentViewModel paymentView)
        {
            var payment = paymentManager.CreatePayment((decimal)paymentView.Amount);

            var approvalUrl = payment.links.FirstOrDefault(link => link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

            if (string.IsNullOrEmpty(approvalUrl))
            {
                return BadRequest(new { message = "Unable to create PayPal payment" });
            }

            return Ok(new { approvalUrl });
        }

        [HttpPost("add-stripe-payment")]
        public IActionResult AddStripePayment([FromBody] PaymentViewModel paymentView)
        {
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = (long)(paymentView.Amount * 100),
                ReceiptEmail = paymentView.StripeEmail,
                Currency = "EGY",
                Description = "Auction bid payment",
                Source = paymentView.Token,
                
            };

            var chargeService = new ChargeService();

            try
            {
                Charge charge = chargeService.Create(chargeOptions); 
                if (charge.Status == "succeeded")
                {
                    return Ok(new { message = "Payment successful", chargeId = charge.Id });
                }
                else
                {
                    return BadRequest(new { message = "Payment failed", status = charge.Status });
                }
            }
            catch (StripeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all-payments")]
        public IActionResult GetAllPayments() => Ok(paymentManager.GetAll().ToList());
        
        [HttpGet("payment{id:int}")]
        public IActionResult Paymenet(int id) => Ok(paymentManager.GetByID(id));

        [HttpPost("add-payment")]
        public IActionResult AddPayment(PaymentViewModel paymentView)
        {
            if(!ModelState.IsValid)
                return BadRequest(new {message="this data is not completed"});
            var res = paymentManager.Add(paymentView);
            if(!res)
                return BadRequest(new { message = "this error in add payment manager" });
            return Created();
        }
        [HttpDelete("payment{id:int}")]
        public IActionResult DeletePayment(int id)
        {

            if(!paymentManager.Delete(id))
                return  BadRequest(new { Message = "can't find this id"});
            return Ok(new { Message = "deleted seccessfuly" });
        }

        [HttpPut("update-payment")]
        public IActionResult DeletePayment(PaymentViewModel paymentView)
        {
            if (!paymentManager.Update(paymentView))
                return BadRequest(new { Message = "can't find this column" });
            return Ok(new { Message = "updated seccessfuly" });
        }

        #region PaymentCancel and PaymentSuccess
        //[HttpGet("payment-success")]
        //public IActionResult PaymentSuccess([FromQuery] string paymentId, [FromQuery] string payerId)
        //{
        //    var payment = paymentManager.ExecutePayment(paymentId, payerId);

        //    if (payment.state == "approved")
        //    {
        //        return Ok(new { message = "Payment successful!" });
        //    }

        //    return BadRequest(new { message = "Payment not approved." });
        //}

        //[HttpGet("payment-cancel")]
        //public IActionResult PaymentCancel()
        //{
        //    return Ok(new { message = "Payment was canceled." });
        //}
        #endregion

    }
}
