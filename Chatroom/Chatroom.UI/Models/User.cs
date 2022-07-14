using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Chatroom.UI.Models
{
    public class User : IdentityUser
    {
        public virtual DateTime? LastLoginTime { get; set; }
        public virtual DateTime RegistrationDate { get; set; }
    }
}
