using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatterBox.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Chat))]
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        [ForeignKey(nameof(Sender))]
        public string SenderId { get; set; }
        public User Sender { get; set; }

        [Required, StringLength(100, ErrorMessage = "Content length can't be more than 100 characters.", MinimumLength = 3)]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
