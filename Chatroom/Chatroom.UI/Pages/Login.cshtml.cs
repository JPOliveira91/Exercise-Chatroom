using Chatroom.UI.Models;
using Chatroom.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chatroom.UI.Pages
{
    //[Authorize]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult OnGet()
        {
            User = new User
            {
                UserName = "",
                PasswordHash = ""
            };
            return Page();
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(User.UserName, User.PasswordHash, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToPage("./Home");
            }

            return Page();
        }
    }
}
