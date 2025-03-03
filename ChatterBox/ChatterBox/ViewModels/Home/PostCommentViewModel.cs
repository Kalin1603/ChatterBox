using System.ComponentModel.DataAnnotations;

namespace ChatterBox.ViewModels.Home
{
    public class PostCommentViewModel
    {
        [Required]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty")]
        [StringLength(200, ErrorMessage = "Comment cannot be longer than 200 characters")]
        public string Content { get; set; }
    }
}
