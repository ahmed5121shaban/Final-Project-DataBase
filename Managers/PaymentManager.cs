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

namespace Managers
{
    public class PaymentManager:MainManager<Final.Payment>
    {
        private readonly IConfiguration configuration;

        public PaymentManager(IConfiguration _configuration,FinalDbContext contextOptions):base(contextOptions)
        {
            configuration = _configuration;
        }

        public IQueryable<Final.Payment> GetAll() => GetAll().AsQueryable();
        public Final.Payment GetByID(int id) => GetAll().FirstOrDefault(p=>p.Id==id);

        public bool Add(PaymentViewModel _paymentView)
        {
            try
            {
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


        public bool AddPayPalPayment(PaymentViewModel paymentView)
        {
            var payment = CreatePayment((decimal)paymentView.Amount);

            var approvalUrl = payment.links.FirstOrDefault(link => link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

            if (string.IsNullOrEmpty(approvalUrl))
            {
                return false;
            }

            return true;
        }

        public bool AddStripePayment(PaymentViewModel paymentView)
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
                    //Ok(new { message = "Payment successful", chargeId = charge.Id })
                    return true;
                }
                else
                {
                    //BadRequest(new { message = "Payment failed", status = charge.Status })
                    return false;
                }
            }
            catch (StripeException ex)
            {
                //BadRequest(new { message = ex.Message })
                return false;
            }
        }

        public PayPal.Api.Payment CreatePayment(decimal _amount)
        {
            var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPal:ClientId"],
                configuration["PayPal:ClientSecret"]).GetAccessToken());

            var payment = new PayPal.Api.Payment
            {
                /*intent = "sale",*/
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount { currency = "EGY", total = _amount.ToString() }
                }
            },

            };
            return payment.Create(apiContext);
        }

        public PayPal.Api.Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPal:ClientId"],
                configuration["PayPal:ClientSecret"]).GetAccessToken());
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new PayPal.Api.Payment { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}

