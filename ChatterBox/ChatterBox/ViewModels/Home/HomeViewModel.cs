using ChatterBox.Models;

namespace ChatterBox.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }

        public string StatusMessage { get; set; }
    }
}
