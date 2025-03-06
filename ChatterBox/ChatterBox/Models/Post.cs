using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatterBox.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Content is required."), 
            MinLength(5, ErrorMessage ="The content cannot be less than 5 symbols."), 
            MaxLength(200, ErrorMessage ="The content cannot exceed 200 symbols.")]
        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string? ImageURL { get; set; }

        public int NumberOfReports { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPrivate { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
