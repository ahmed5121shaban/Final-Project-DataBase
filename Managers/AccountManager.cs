using Final;
using Microsoft.AspNetCore.Identity;
using ModelView;
using ModelView.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class AccountManager :MainManager<User>
    {
         private UserManager<User> userManager;
         private SignInManager<User> signInManager;
        private readonly TokenManager tokenManager;

        public AccountManager(FinalDbContext _finalDbContext ,
            UserManager<User> _userManager,
            SignInManager<User> _signInManager,
            TokenManager _tokenManager
            ) : base(_finalDbContext)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            tokenManager = _tokenManager;
        }
        public async Task<IdentityResult> Register(RegisterViewModel viewModel)
        {
            try
            {

                User user = viewModel.ToModel();
                var result = await userManager.CreateAsync(user, viewModel.Password);
                result = await userManager.AddToRolesAsync(user,new List<string>{"User","Buyer"});
                return result;
            }
            catch(Exception ex)
            {
                return new IdentityResult();
            }
        }

        public async Task<string> Login(LoginViewModel viewModel)
        {
            var user = await userManager.FindByEmailAsync(viewModel.Email);
            if (user == null)
            {
                    return string.Empty;
                
            }
            await signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RemeberMe, true);
            return await tokenManager.GenerateToken(user);
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
            if (!string.IsNullOrWhiteSpace(model.FirstName))
                user.Name = model.FirstName;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                user.Name = model.LastName;

            if (!string.IsNullOrWhiteSpace(model.City))
                user.City = model.City;

            if (!string.IsNullOrWhiteSpace(model.Country))
                user.Country = model.Country;

            if (!string.IsNullOrWhiteSpace(model.Street))
                user.Street = model.Street;

            if (!string.IsNullOrWhiteSpace(model.PostalCode))
                user.PostalCode = model.PostalCode;

            if (!string.IsNullOrWhiteSpace(model.Description))
                user.Description = model.Description;

            if (model.Age > 0)
                user.Age = model.Age;

            if (model.NationalId > 0)
                user.NationalId = model.NationalId;

            if (!string.IsNullOrWhiteSpace(model.TimeZone))
                user.TimeZone = model.TimeZone;

            if (model.Gender != null)
                user.Gender = model.Gender;

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
    }
}
