using Chatroom.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chatroom.UI.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        
        public LoginModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager
            )
        {            
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult OnGet()
        {
            LoginUser = new User
            {
                UserName = "",
                PasswordHash = ""
            };

            if (User.Identity.IsAuthenticated) return RedirectToPage("./Home");

            return Page();
        }

        [BindProperty]
        public User LoginUser { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(LoginUser.UserName, LoginUser.PasswordHash, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var updateResult = await UpdateLastLoginDate(LoginUser);

                return updateResult ? RedirectToPage("./Home") : RedirectToPage("./Error");
            }

            return Page();
        }

        public async Task<Boolean> UpdateLastLoginDate(User user)
        {
            var loggedUser = await _userManager.FindByNameAsync(user.UserName);

            loggedUser.LastLoginTime = DateTime.Now;

            var result = await _userManager.UpdateAsync(loggedUser);

            return result == IdentityResult.Success;
        }
    }
}
