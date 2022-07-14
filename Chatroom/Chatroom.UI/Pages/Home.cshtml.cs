using Chatroom.UI.Data;
using Chatroom.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Chatroom.UI.Pages
{
    [Authorize]
    public class HomeModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ChatroomUIContext _context;

        public HomeModel(
            UserManager<User> userManager,
            ChatroomUIContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<Message> Messages { get; set; } = new List<Message>();

        [BindProperty]
        public Message Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Message = new Message
            {
                Text = string.Empty
            };

            var result = await RetrieveMessages();

            return result ? Page() : RedirectToPage("./Error");
        }

        private async Task<Boolean> RetrieveMessages()
        {
            var user = await RetrieveUser();

            IQueryable<Message> messages = from m in _context.Message
                                           where m.Date >= user.LastLoginTime
                                           orderby m.Date descending
                                           select m;

            Messages = await messages.Take(50).ToListAsync();

            return true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Message.Text.StartsWith("/stock="))
            {
                var stockCode = Message.Text.Replace("/stock=", "");

                Message.Text = string.Format("{0} quote is 123.45 per share", stockCode);
            }

            var saveResult = await SaveMessage(Message);

            if (saveResult)
            {
                var result = await RetrieveMessages();

                return result ? Page() : RedirectToPage("./Error");
            }
            else
            {
                return RedirectToPage("./Error");
            }
        }

        public async Task<Boolean> SaveMessage(Message message)
        {            
            message.UserName = User.Identity.Name;
            message.Date = DateTime.Now;

            _context.Message.Add(message);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        private async Task<User> RetrieveUser()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
