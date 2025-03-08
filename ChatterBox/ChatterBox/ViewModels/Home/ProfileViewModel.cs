using ChatterBox.Models;
using System.Collections.Generic;

namespace ChatterBox.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}