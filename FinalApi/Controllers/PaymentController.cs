using FinalApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using Managers;
using Stripe;
using System.Security.Claims;
using PayPal.Api;
using PayPal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.SignalR;

namespace FinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;
        private readonly PaymentManager paymentManager;
        private readonly AuctionManager auctionManager;
        private readonly IHubContext<DashboardHub> hubContext;
        private readonly IHubContext<ProfileHub> profileHubContext;
        private readonly IHubContext<NotificationsHub> notificationHub;
        private readonly NotificationManager notificationManager;
        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>
        {
            { "mode", "sandbox" },
        };

        public PaymentController(IConfiguration _configuration, UserManager<User> _userManager, PaymentManager _paymentManager,
            AuctionManager _auctionManager,IHubContext<DashboardHub> _hubContext,IHubContext<ProfileHub> _profileHubContext,
            IHubContext<NotificationsHub> _notificationHub,NotificationManager _notificationManager)
        {
            configuration = _configuration;
            userManager = _userManager;
            paymentManager = _paymentManager;
            auctionManager = _auctionManager;
            hubContext = _hubContext;
            profileHubContext = _profileHubContext;
            notificationHub = _notificationHub;
            notificationManager = _notificationManager;
        }


        [HttpGet("all-payments")]
        public IActionResult GetAllPayments() => Ok(paymentManager.GetAll().ToList());

        [HttpGet("payment{id:int}")]
        public IActionResult Payment(int id) => Ok(paymentManager.GetByID(id));


        [HttpGet("payment-for-buyer")]
        public IActionResult PaymentForBuyer()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userID))
                return BadRequest(new { message = "user not found" });
            var payment = paymentManager.GetAll().FirstOrDefault(p => p.BuyerId == userID && p.AuctionID != null);

            if (payment == null)
                return new JsonResult(new { statusCode = 400, result = payment?.AuctionID??-1, message = "the payment not found" });
            
            return new JsonResult(new { statusCode = 200, result = payment.AuctionID, message = "the payment found successfully" });
        }

        [HttpPost("add-payment")]
        public async Task<IActionResult> AddPayment(AddPaymentViewModel paymentView)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "this data is not completed" });
            var res =await paymentManager.Add(paymentView);
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

            if (user == null)return NotFound("User not found");

            if(paymentView.Method== Enums.PaymentMetod.paypal)
            {
                user.PaypalEmail = paymentView.PaypalEmail;
                await profileHubContext.Clients.All.SendAsync("paypalEmail", user.PaypalEmail);
            }
            if (paymentView.Method == Enums.PaymentMetod.stripe)
            {
                user.StripeEmail = paymentView.StripeEmail;
                await profileHubContext.Clients.All.SendAsync("stripeEmail", user.StripeEmail);
            }

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
            
            var email = userManager.Users.Where(u => u.Id == userid).Select(u=>new { u.PaypalEmail, u.StripeEmail}).ToList();
            if (!email.Any()) 
                return NotFound(new {message="No Emails in this account"});
            
           
            return Ok(email);
        }

        [HttpPost("send")]
        public IActionResult SendPayout(string recipientEmail, decimal totalAmount)
        {
            try
            {
                var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPalSetting:ClientID"],
                configuration["PayPalSetting:Secret"],Config).GetAccessToken());

                var senderBatchId = Guid.NewGuid().ToString();

                var payoutItem = new PayoutItem
                {
                    recipient_type = PayoutRecipientType.EMAIL,
                    amount = new Currency
                    {
                        value = totalAmount.ToString("F2"),
                        currency = "EUR"
                    },
                    receiver = recipientEmail,
                    note = $"Payment after deducting {totalAmount}% fee",
                    sender_item_id = "item_" + Guid.NewGuid().ToString()
                };


                var payout = new PayPal.Api.Payout
                {
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = senderBatchId,
                        email_subject = "You have a payment"
                    },
                    items = new List<PayoutItem> { payoutItem }
                };

                var createdPayout = payout.Create(apiContext, syncMode: true);

                return Ok(new
                {
                    status = createdPayout.batch_header.batch_status,
                    batchId = createdPayout.batch_header.payout_batch_id,
                    statusCode = 200
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message, statusCode = 400 });
            }
        }

        [HttpPost("create-paypal-payment")]
        public async Task<IActionResult> CreatePayPalPayment([FromBody] CreatePaymentViewModel _createPayment)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var buyerID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerID))
                return BadRequest(new { message = "the user not found" });

            var user = userManager.FindByIdAsync(buyerID).Result.City;
            if (string.IsNullOrEmpty(user))
                return Ok(new { result = "http://localhost:4200/user/shipping", status = 400 });

            try
            {
                var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPalSetting:ClientID"],
                    configuration["PayPalSetting:Secret"], Config).GetAccessToken());

                var payment = new PayPal.Api.Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        total = _createPayment.Amount.ToString("F2"),
                        currency = "USD",
                    },
                    description = "Auction deposit"
                }
            },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = $"http://localhost:4200/user/won-auction?auctionId={_createPayment.auctionID}",
                        cancel_url = "http://localhost:4200/user/won-auction"
                    }
                };

                var createdPayment = payment.Create(apiContext).GetApprovalUrl();
                if (string.IsNullOrEmpty(createdPayment))
                    return BadRequest(new { message = "adding payment is not completed" });

                await hubContext.Clients.All.SendAsync("auctionAmount", _createPayment.Amount);
               
                return Ok(new { result = createdPayment, status = 200 }); 
                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "adding payment is not completed" });
            }
            
        }

        [HttpGet("auction/success")]
        public async Task<IActionResult> AuctionSuccess(string paymentId, string payerId,int auctionId)
        {
            var buyerID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerID))
                return BadRequest(new { message = "the user not found" ,statusCode = 404});
            try
            {
                var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPalSetting:ClientID"],
                    configuration["PayPalSetting:Secret"], Config).GetAccessToken());

                var paymentExecution = new PaymentExecution { payer_id = payerId };
                var payment = new PayPal.Api.Payment { id = paymentId };
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state.ToLower() == "approved")
                {
                    var auction = auctionManager.GetAll().FirstOrDefault(a=>a.BuyerID== buyerID &&a.ID== auctionId);
                    if (auction == null)
                        return BadRequest(new { message = "the payment not completed", statusCode = 404 });
                    auction.Completed=true;
                    auction.ShippingStatus = Enums.AuctionShippingStatus.Pending;
                    if(!await auctionManager.Update(auction))
                        return BadRequest(new { message = "the payment not completed", statusCode = 400 });
                    var sellerID = auctionManager.Get(auctionId).Result.Item.Seller.UserID;
                    var notificationName = auctionManager.Get(auctionId).Result.Item.Name;
                    await notificationManager.Add(new Notification
                    {
                        Date = DateTime.Now,
                        Description = $"your Auction {notificationName} Payment is Completed",
                        IsReaded = false,
                        Title = Enums.NotificationType.auction,
                        UserId = sellerID,

                    });
                    var lastNotification = notificationManager.GetAll().Where(n => n.UserId == sellerID).OrderBy(n => n.Id).LastOrDefault();
                    await notificationHub.Clients.Group(sellerID).SendAsync("notification", lastNotification.ToViewModel());
                    await hubContext.Clients.All.SendAsync("completedAuction",1);
                    return Ok(new { message = "Payment successful", statusCode = 200 });
                }

                return BadRequest(new {message = "Payment not approved.", statusCode = 400 });
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
        public async Task<IActionResult> CreateStripePayment([FromBody] CreatePaymentViewModel _createPayment)
        {
            if(!ModelState.IsValid)
                return BadRequest(new {message = "this data is not completed"});

            var buyerID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerID))
                return BadRequest(new { message = "the user not found" });
            var user = userManager.FindByIdAsync(buyerID).Result.City;
            if (string.IsNullOrEmpty(user))
                return BadRequest(new { result= "http://localhost:4200/user/shipping", status = 400 });
            try
            {
                StripeConfiguration.ApiKey = configuration["StripeSetting:SecretKey"];

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            {
                new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {

                        UnitAmount = (long)_createPayment.Amount * 100,
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "ProductName",
                        },
                    },
                    Quantity = 1,
                }
            },
                    Mode = "payment",
                    SuccessUrl = $"http://localhost:4200/user/won-auction?auctionId={_createPayment.auctionID}&success=1",
                    CancelUrl = "http://localhost:4200/user/won-auction?success=0",
                };


                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);

                //send the total amount to dashboard and is completed
                //send notification for seller (check this)
                var sellerID = auctionManager.Get(_createPayment.auctionID).Result.Item.Seller.UserID;
                var notificationName = auctionManager.Get(_createPayment.auctionID).Result.Item.Name;
                await notificationManager.Add(new Notification
                {
                    Date = DateTime.Now,
                    Description = $"your Auction {notificationName} Payment is Completed",
                    IsReaded = false,
                    Title = Enums.NotificationType.auction,
                    UserId = sellerID,

                });
                var lastNotification = notificationManager.GetAll().Where(n=>n.UserId == sellerID).OrderBy(n=>n.Id).LastOrDefault();
                await notificationHub.Clients.Group(sellerID).SendAsync("notification", lastNotification.ToViewModel());
                await hubContext.Clients.All.SendAsync("auctionAmount", _createPayment.Amount);
                await hubContext.Clients.All.SendAsync("completedAuction", 1);
                var result = session.Url;
                if (!string.IsNullOrEmpty(result))
                {
                    var auction = auctionManager.GetAll().FirstOrDefault(a => a.BuyerID == buyerID && a.ID == _createPayment.auctionID);
                    if (auction == null)
                        return BadRequest(new { message = "the payment not completed", statusCode = 404 });
                    auction.Completed = true;
                    auction.ShippingStatus = Enums.AuctionShippingStatus.Pending;
                    if (!await auctionManager.Update(auction))
                        return BadRequest(new { message = "the payment not completed", statusCode = 400 });
                }

                return Ok(new { result, status = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "the client Secret is not Created" });
            }

            
        }


        //this api in bid controller not here

        [HttpGet("user-have-payment")]
        public IActionResult GetPaymentMethodsCount()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
                return new JsonResult(new { message = "the user not found", count = 404 });
        

            var payments = paymentManager.GetAll().Where(p => p.BuyerId == userID).ToList();

            if (payments.Count == 0)
                return new JsonResult(new { message = "the user don't have any Payment Method", count = 0 });
            else if (payments.Count > 1)
                return new JsonResult(new { message = "the user have more than one Payment Method", count = 2 });
            else
                return new JsonResult(new { message = "the user have one Payment Method", count = 1, method = payments.Select(p => p.Method) });

        }
    }

   
}

