using Chatroom.UI.Data;
using Chatroom.UI.Models;
using Chatroom.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chatroom.UI.Pages
{
    //[AllowAnonymous]
    public class NewUserModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ChatroomUIContext _context;
        private readonly SignInManager<User> _signInManager;

        public NewUserModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult OnGet()
        {
            User  = new User
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
                return RedirectToPage("./NewUser");
            }

            var user = new User { UserName = User.UserName };
            var result = await _userManager.CreateAsync(user, User.PasswordHash);
            if (result.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("./Home");
            }

            return RedirectToPage("./NewUser");
        }
    }
}
