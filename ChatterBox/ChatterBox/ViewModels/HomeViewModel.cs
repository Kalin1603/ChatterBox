using ChatterBox.Models;

namespace ChatterBox.ViewModels
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }

        public List<Post> Stories { get; set; }

        public string StatusMessage { get; set; }
    }
}
