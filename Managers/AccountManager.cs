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
        public AccountManager(FinalDbContext _finalDbContext , UserManager<User> _userManager, SignInManager<User> _signInManager) : base(_finalDbContext)
        {
            userManager = _userManager;
            signInManager = _signInManager;

        }
        public async Task<IdentityResult> SignUp(RegisterViewModel viewModel)
        {
            User user = viewModel.ToModel();
            var result = await userManager.CreateAsync(user, viewModel.Password);
            result = await userManager.AddToRoleAsync(user, viewModel.Role);
            return result;
        }

        public async Task<SignInResult> SignIn(LoginViewModel viewModel)
        {
            var user = await userManager.FindByEmailAsync(viewModel.Email);
            if (user == null)
            {
                    return SignInResult.Failed;
                
            }

            return await signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RemeberMe, true);
        }
        public async void SignOut()
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
    }
}
