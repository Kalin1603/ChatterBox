using ChatterBox.Models;

namespace ChatterBox.ViewModels.UserViewModels
{
    public class SuggestedUserViewModel
    {
        public User User { get; set; }
        public bool IsFollowed { get; set; }
    }
}
