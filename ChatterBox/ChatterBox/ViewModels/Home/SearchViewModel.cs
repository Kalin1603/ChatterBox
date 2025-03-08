using ChatterBox.Models;

namespace ChatterBox.ViewModels.Home
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<User> Users { get; set; } = new List<User>(); 
    }
}