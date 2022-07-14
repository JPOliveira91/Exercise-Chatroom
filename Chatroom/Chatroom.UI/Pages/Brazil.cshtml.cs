using Chatroom.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chatroom.UI.Pages
{
    [Authorize]
    public class BrazilModel : PageModel
    {
        public BrazilModel()
        {
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

            return Page();
        }
    }
}
