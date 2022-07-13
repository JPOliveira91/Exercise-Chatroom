using Chatroom.UI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chatroom.UI.Data
{
    public class ChatroomUIContext : IdentityDbContext<User>
    {
        public ChatroomUIContext(DbContextOptions<ChatroomUIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Message>? Message { get; set; }
        public DbSet<Models.User>? User { get; set; }
    }
}
