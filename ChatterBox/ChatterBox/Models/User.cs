using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChatterBox.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Full name is required"), 
            MaxLength(50, ErrorMessage = "Full name cannot exceed 50 symbols"), 
            MinLength(3, ErrorMessage = "Full name cannot be less than 3 symbols")]
        public string FullName { get; set; }

        public string? ProfilePictureURL { get; set; }

        [Required(ErrorMessage = "Country is required"), 
            MaxLength(50, ErrorMessage = "Country cannot exceed 50 symbols"), 
            MinLength(3, ErrorMessage = "Country cannot be less than 3 symbols")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City is required"), 
            MaxLength(50, ErrorMessage = "City cannot exceed 50 symbols"), 
            MinLength(3, ErrorMessage = "City cannot be less than 3 symbols")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required"), 
            MaxLength(50, ErrorMessage = "State cannot exceed 50 symbols"), 
            MinLength(3, ErrorMessage = "State cannot be less than 3 symbols")]
        public string State { get; set; }

        public string Address { get; set; }

        public string Zip { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime DateOfBirth { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Story> Stories { get; set; } = new List<Story>();

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
