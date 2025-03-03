using System.ComponentModel.DataAnnotations;

namespace ChatterBox.Models
{
    public class Hashtag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage ="Hashtag name cannot be more than 50 symbols.")]
        public string Name { get; set; }

        public int Count { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
