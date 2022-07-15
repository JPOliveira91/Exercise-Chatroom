using System.ComponentModel.DataAnnotations;

namespace Chatroom.UI.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public string Room { get; set; }

        public string UserName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
