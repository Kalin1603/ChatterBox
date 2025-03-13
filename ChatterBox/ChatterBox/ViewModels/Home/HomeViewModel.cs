using ChatterBox.Models;
using ChatterBox.ViewModels.UserViewModels;

namespace ChatterBox.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }

        public string StatusMessage { get; set; }

        public List<SuggestedUserViewModel> PeopleYouMayKnow { get; set; } = new();
    }
}
