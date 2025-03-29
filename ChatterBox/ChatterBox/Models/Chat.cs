using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatterBox.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Initiator))]
        public string InitiatorId { get; set; } 
        public User Initiator { get; set; }

        [ForeignKey(nameof(Recipient))]
        public string RecipientId { get; set; } 
        public User Recipient { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
