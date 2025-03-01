using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatterBox.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public Post Post { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
