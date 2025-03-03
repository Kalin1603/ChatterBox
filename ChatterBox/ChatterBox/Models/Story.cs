using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatterBox.Models
{
    public class Story
    {
        [Key]
        public int Id { get; set; }

        public string? ImageURL { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
