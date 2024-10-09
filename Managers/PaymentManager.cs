using Final;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModelView;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Stripe;
using PayPal;

namespace Managers
{
    public class PaymentManager:MainManager<Final.Payment>
    {
        private readonly IConfiguration configuration;
        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>
        {
            { "mode", "sandbox" } 
        };

        public PaymentManager(IConfiguration _configuration,FinalDbContext contextOptions):base(contextOptions)
        {
            configuration = _configuration;
        }

        public IQueryable<Final.Payment> GetAll() => base.GetAll().AsQueryable();
        public Final.Payment GetByID(int id) => GetAll().FirstOrDefault(p=>p.Id==id);

        public bool Add(AddPaymentViewModel _paymentView)
        {
            try
            {
                var res = GetAll().Where(p=>p.BuyerId==_paymentView.BuyerId&&p.Method==_paymentView.Method).FirstOrDefault();
                if (res != null) return false;
                Add(_paymentView.ToModel());
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }

           
        }

        public bool Delete(int id)
        {
            try
            {
                var payment = GetByID(id);
                if (payment == null)
                    return false;
                Delete(payment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public bool Update(PaymentViewModel _paymentView)
        {
            try
            {
                Update(_paymentView.ToModel());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }



        public string AddPayPalPayment(CreatePaymentViewModel _createPayment)
        {
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
                        currency = _createPayment.Currency,
                    },
                    description = "Auction deposit"
                }
            },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = "http://localhost:5204/api/Payment/auction/success",
                        cancel_url = "http://localhost:5204/api/Payment/auction/cancel"
                    }
                };

                var createdPayment = payment.Create(apiContext);
                return createdPayment.GetApprovalUrl();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string AddStripePayment(CreatePaymentViewModel _createPayment)
        {
            try
            {
                // Set the API key from the configuration
                StripeConfiguration.ApiKey = configuration["StripeSetting:SecretKey"];

                // Create session options for Stripe Checkout
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" }, // Specify accepted payment methods
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            {
                new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)_createPayment.Amount * 100, // Convert amount to cents
                        Currency = "usd", // Use USD for the payment
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "ProductName", // Get the product name from the model
                        },
                    },
                    Quantity = 1,
                }
            },
                    Mode = "payment", // Payment session mode
                    SuccessUrl = "https://yourdomain.com/success?session_id={CHECKOUT_SESSION_ID}", // Redirect on success
                    CancelUrl = "https://yourdomain.com/cancel", // Redirect on cancel
                };

                // Create the session service and generate a new session
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);

                // Return the session URL for redirecting the user to the Stripe Checkout page
                return session.Url;
            }
            catch (Exception ex)
            {
                // Handle any errors that occur and return an empty string if unsuccessful
                return string.Empty;
            }

        }


        public dynamic RefundCustomer(APIContext apiContext, string customerEmail, decimal refundAmount)
        {
            try
            {
                // Create another payout item for the refund
                var refundItem = new PayoutItem
                {
                    recipient_type = PayoutRecipientType.EMAIL,
                    amount = new Currency
                    {
                        value = refundAmount.ToString("F2"),
                        currency = "USD"
                    },
                    receiver = customerEmail,
                    note = "Refund of deducted fee",
                    sender_item_id = "refund_item_" + Guid.NewGuid().ToString()
                };

                // Create payout batch for the refund
                var refundPayout = new PayPal.Api.Payout
                {
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        email_subject = "Refund of your deducted amount"
                    },
                    items = new List<PayoutItem> { refundItem }
                };

                // Send the refund payout to the customer
                var createdRefundPayout = refundPayout.Create(apiContext);

                // Return the refund result
                return new
                {
                    status = createdRefundPayout.batch_header.batch_status,
                    amount = refundAmount
                };
            }
            catch (PayPalException ex)
            {
                // Log and handle any refund-related exceptions
                return new { status = "FAILED", message = ex.Message };
            }
        }

    }
}

