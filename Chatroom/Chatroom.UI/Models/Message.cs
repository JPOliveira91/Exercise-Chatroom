using System.ComponentModel.DataAnnotations;

namespace Chatroom.UI.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int UserId { get; set; }
    }
}
