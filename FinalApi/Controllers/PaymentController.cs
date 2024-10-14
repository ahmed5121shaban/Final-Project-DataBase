using Final;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using Managers;
using Stripe;
using System.Security.Claims;
using PayPal.Api;
using PayPal;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;
        private readonly PaymentManager paymentManager;
        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>
        {
            { "mode", "sandbox" },
        };

        public PaymentController(IConfiguration _configuration, UserManager<User> _userManager, PaymentManager _paymentManager)
        {
            configuration = _configuration;
            userManager = _userManager;
            paymentManager = _paymentManager;
        }


        [HttpGet("all-payments")]
        public IActionResult GetAllPayments() => Ok(paymentManager.GetAll().ToList());

        [HttpGet("payment{id:int}")]
        public IActionResult Payment(int id) => Ok(paymentManager.GetByID(id));


        [HttpGet("payment-for-buyer")]
        public IActionResult PaymentForBuyer()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payment = paymentManager.GetAll().FirstOrDefault(p => p.BuyerId == userID&&p.AuctionID!=null);
            if (payment == null)
                return BadRequest(new { message="this payment is not found"});
            return Ok(payment);
        }

        [HttpPost("add-payment")]
        public IActionResult AddPayment(AddPaymentViewModel paymentView)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "this data is not completed" });
            var res = paymentManager.Add(paymentView);
            if (!res)
                return BadRequest(new { message = "this error in add payment manager" });
            return Created();
        }
        [HttpDelete("payment{id:int}")]
        public IActionResult DeletePayment(int id)
        {

            if (!paymentManager.Delete(id))
                return BadRequest(new { Message = "can't find this id" });
            return Ok(new { Message = "deleted seccessfuly" });
        }

        [HttpPut("update-payment")]
        public IActionResult UpdatePayment(PaymentViewModel paymentView)
        {
            if (!paymentManager.Update(paymentView))
                return BadRequest(new { Message = "can't find this column" });
            return Ok(new { Message = "updated seccessfuly" });
        }


        [HttpPost("add-payment-email")]
        public async Task<IActionResult> AddPaymentEmail([FromBody] PaymentViewModel paymentView)
        {
            
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if(paymentView.Method== Enums.PaymentMetod.paypal)
            {
                user.PaypalEmail = paymentView.PaypalEmail;
            }
            if (paymentView.Method == Enums.PaymentMetod.stripe)
            {
                user.StripeEmail = paymentView.StripeEmail;
            }

            if(!paymentManager.Add(new AddPaymentViewModel 
            { 
                 BuyerId = userid, Method = paymentView.Method 
            }
            ))
                return BadRequest(new { message = "Not Added in Payment Table." });

            var res = await userManager.UpdateAsync(user);
            if(!res.Succeeded)
                return BadRequest(new { message = "Payment Email is not added." });

            return Ok(new { message = "Payment Email added Successfully." });
        }

        [HttpGet("get-payment-email")]
        public IActionResult GetPaymentEmail()
        {

            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null)
                return NotFound(new { message = "User not found" });
            
            var email = userManager.Users.Where(u => u.Id == userid).Select(u=>new { PaypalEmail=u.PaypalEmail, StripeEmail=u.StripeEmail}).ToList();
            if (!email.Any()) 
                return NotFound(new {message="No Emails in this account"});
            
            
            return Ok(email);
        }

        [HttpPost("send")]
        public IActionResult SendPayout(string recipientEmail, decimal totalAmount)
        {
            try
            {
                // Initialize PayPal API context
                var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPalSetting:ClientID"],
               configuration["PayPalSetting:Secret"],Config).GetAccessToken());

                // Deduct a percentage (e.g., 10%)
                //decimal deductionPercentage = 0.10m;
                //decimal amountToDeduct = totalAmount * deductionPercentage;
                //decimal amountToSend = totalAmount - amountToDeduct;
                decimal amountToSend = 500m;

                // Create a sender batch ID
                var senderBatchId = Guid.NewGuid().ToString();

                // Create payout item for the recipient (after deduction)
                var payoutItem = new PayoutItem
                {
                    recipient_type = PayoutRecipientType.EMAIL,
                    amount = new Currency
                    {
                        value = amountToSend.ToString("F2"),
                        currency = "EUR"
                    },
                    receiver = recipientEmail,
                    note = $"Payment after deducting {amountToSend}% fee",
                    sender_item_id = "item_" + Guid.NewGuid().ToString()
                };

                // Create the payout batch request
                var payout = new PayPal.Api.Payout
                {
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = senderBatchId,
                        email_subject = "You have a payment"
                    },
                    items = new List<PayoutItem> { payoutItem }
                };

                // Send the payout to the recipient
                var createdPayout = payout.Create(apiContext, syncMode: true);

                // After successful payout, return the deducted amount back to the customer
                string customerEmail = "customer@example.com";  // Customer's PayPal email
                //var refundResult = paymentManager.RefundCustomer(apiContext, customerEmail, amountToDeduct);

                // Return a success response with details
                return Ok(new
                {
                    status = createdPayout.batch_header.batch_status,
                    batchId = createdPayout.batch_header.payout_batch_id,
                    //refundedAmount = refundResult.amount
                });
            }
            catch (Exception ex)
            {
                // Handle errors (like insufficient funds or invalid recipients)
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-paypal-payment")]
        public IActionResult CreatePayPalPayment([FromBody] CreatePaymentViewModel _createPayment)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            string approvalUrl = paymentManager.AddPayPalPayment(_createPayment);
            if (string.IsNullOrEmpty(approvalUrl))
                return BadRequest(new { message = "adding payment is not completed" });
            return Ok(new { urlCheckOut = approvalUrl, status=200 });
        }

        [HttpGet("auction/success")]
        public IActionResult AuctionSuccess(string paymentId, string payerId)
        {
            try
            {
                var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPalSetting:ClientID"],
                    configuration["PayPalSetting:Secret"], Config).GetAccessToken());

                var paymentExecution = new PaymentExecution { payer_id = payerId };
                var payment = new PayPal.Api.Payment { id = paymentId };
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state.ToLower() == "approved")
                {
                    return Ok(new { status = "Payment successful", statusCode = 200 });
                }

                return BadRequest("Payment not approved.");
            }
            catch (PaymentsException ex)
            {
                var errorResponse = ex.Response;
                var debugId = ex.Details.debug_id;
                var errorName = ex.Details.name;

                if (errorName == "COMPLIANCE_VIOLATION")
                {
                    Console.WriteLine($"Compliance violation error: {errorResponse}");
                    Console.WriteLine($"Debug ID: {debugId}");

                    return BadRequest(new
                    {
                        message = "Transaction declined due to compliance violation. Please contact support.",
                        errorName = errorName,
                        debugId = debugId,
                        statusCode = 400
                    });
                }

                return BadRequest(new
                {
                    message = "An error occurred while processing your payment.",
                    errorName = errorName,
                    debugId = debugId,
                    statusCode = 400
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-stripe-payment")]
        public IActionResult CreateStripePayment([FromBody] CreatePaymentViewModel _createPayment)
        {
            if(!ModelState.IsValid)
                return BadRequest(new {message = "this data is not completed"});
            string clientSecret = paymentManager.AddStripePayment(_createPayment);
            if (string.IsNullOrEmpty(clientSecret))
                return BadRequest(new { message = "the client Secret is not Created" });
            return Ok(new { clientSecret });
        }
    }

   
}

