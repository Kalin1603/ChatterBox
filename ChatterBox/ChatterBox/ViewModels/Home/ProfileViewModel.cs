using ChatterBox.Models;
using ChatterBox.ViewModels.UserViewModels;
using System.Collections.Generic;

namespace ChatterBox.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public User User { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();

        public bool IsFollowed { get; set; }

        public bool IsFollowPending { get; set; }

        public List<SuggestedUserViewModel> PeopleYouMayKnow { get; set; } = new List<SuggestedUserViewModel>();

        public List<SuggestedUserViewModel> Followers { get; set; } = new List<SuggestedUserViewModel>(); 

        public List<SuggestedUserViewModel> Followings { get; set; } = new List<SuggestedUserViewModel>();

        public string StatusMessage { get; set; }
    }
}