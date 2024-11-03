using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ModelView;
using ModelView.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalApi;

using ModelView;
using FinalApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Managers
{
    public class AccountManager : MainManager<User>
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private readonly TokenManager tokenManager;
        private SellerManager sellerManager;
        private BuyerManager buyerManager;
        public AccountManager(FinalDbContext _finalDbContext,
            UserManager<User> _userManager,
            SignInManager<User> _signInManager,
            TokenManager _tokenManager,
            SellerManager _sellerManager,
            BuyerManager _buyerMamanger
            ) : base(_finalDbContext)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            tokenManager = _tokenManager;
            sellerManager= _sellerManager;
            buyerManager = _buyerMamanger;
        }
        public UserManager<User> UserManager => userManager;
        public async Task<IdentityResult> Register(RegisterViewModel viewModel)
        {
            try
            {

                User user = viewModel.ToModel();
                var result = await userManager.CreateAsync(user, viewModel.Password);
                //result = await userManager.AddToRolesAsync(user, new List<string> { "User", "Admin" });
                result = await userManager.AddToRolesAsync(user, new List<string> { "User", "Buyer" });
                var res = buyerManager.Add(new Buyer
                {
                    User = user,
                    UserID = user.Id
                });
                return result;
            }
            catch (Exception ex)
            {
                return new IdentityResult();
            }
        }

        public async Task<string> Login(LoginViewModel viewModel)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(viewModel.Email);
                if (user == null)
                {
                    return string.Empty;
                }
                var password = await signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RemeberMe, true);
                if (!password.Succeeded)
                    return string.Empty;
                return await tokenManager.GenerateToken(user);
            }catch (Exception ex)
            {
                return string.Empty;
            }
           
        }
        public async void Logout()
        {
            await signInManager.SignOutAsync();

        }

        public async Task<IdentityResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var User = await userManager.FindByIdAsync(viewModel.UserID);

            return await userManager.ChangePasswordAsync(User, viewModel.CurrentPassword, viewModel.NewPassword);
        }
        public async Task<string> GetResetPasswordCode(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return string.Empty;
            }
            else
            {
                return await userManager.GeneratePasswordResetTokenAsync(user);
            }
        }

        public async Task<IdentityResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            var user = await userManager.FindByEmailAsync(viewModel.Email);
            if (user != null)
            {
                return await userManager.ResetPasswordAsync
                     (user, viewModel.Code, viewModel.NewPassword);
            }
            else
            {
                return IdentityResult.Failed(
                    new IdentityError()
                    {
                        Description = "Invalid"
                    });
            }
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(User user, UpdateProfileViewModel model)
        {
            // Check if each field is provided and update it accordingly
            // التأكد من عدم وجود فراغات أو تكرار الاسم
            if (!string.IsNullOrWhiteSpace(model.FirstName) || !string.IsNullOrWhiteSpace(model.LastName))
            {
                user.Name = $"{model.FirstName?.Trim()} {model.LastName?.Trim()}".Trim();
            }

       

            if (!string.IsNullOrWhiteSpace(model.Description))
                user.Description = model.Description;

            if (model.Age > 0)
                user.Age = model.Age;


            if (!string.IsNullOrWhiteSpace(model.TimeZone))
                user.TimeZone = model.TimeZone;
            if (!string.IsNullOrWhiteSpace(model.Currency))
                user.Currency = model.Currency;
            if (model.Gender != null)
                user.Gender = model.Gender;

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                user.Email = model.Email;
                user.UserName = model.Email;
            }
            // Update phone numbers if provided
            if (model.PhoneNumbers != null && model.PhoneNumbers.Any())
            {
                user.PhoneNumbers.Clear(); // Clear existing numbers
                foreach (var phone in model.PhoneNumbers)
                {
                    user.PhoneNumbers.Add(new PhoneNumber { Phone = phone, UserID = user.Id });
                }
            }

            // Save changes to the database
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Optionally re-sign-in the user if any sensitive information (like email) was updated
                await signInManager.RefreshSignInAsync(user);
            }

            return result;
        
        }

        public async Task<IdentityResult> UpdateUserAddressAsync(User user, UpdateAddressViewModel model)
        {
          

            if (!string.IsNullOrWhiteSpace(model.City))
                user.City = model.City;

            if (!string.IsNullOrWhiteSpace(model.Country))
                user.Country = model.Country;

            if (!string.IsNullOrWhiteSpace(model.Street))
                user.Street = model.Street;

            if (!string.IsNullOrWhiteSpace(model.PostalCode))
                user.PostalCode = model.PostalCode;


            // Save changes to the database
            var result = await userManager.UpdateAsync(user);

            return result;

        }

        public async Task CheckIfSeller(User user)
        {
            //get roles of user
            try { 
            var roleList = await userManager.GetRolesAsync(user);
            //if user is not seller
            if (!roleList.Contains("Seller"))
            {
                //add seller role to it
                var result = await userManager.AddToRolesAsync(user, new List<string> { "seller" });
                //and add seller row in seller table
                if (result.Succeeded)
                {
                    var res = await sellerManager.Add(new Seller
                    {
                        UserID = user.Id,
                        User = user,
                        WithdrawnAmount = (decimal)0.00
                    });
                }
            }
            }catch(Exception ex) { }
        }


        public async Task<IdentityResult> VerifyIdentity(string userId, string token)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError()
                    {
                        Description = "User not found"
                    });
                }

                // Verifying the token provided by the user
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    // Optionally, mark the user as verified in other ways or update specific claims
                }

                return result;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Description = $"An error occurred: {ex.Message}"
                });
            }
        }

    }
}
