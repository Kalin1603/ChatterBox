using ChatterBox.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatterBox.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Receiver))]
        public string ReceiverId { get; set; }
        public User Receiver { get; set; }

        [ForeignKey(nameof(Sender))]
        public string SenderId { get; set; }
        public User Sender { get; set; }

        public NotificationType Type { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
