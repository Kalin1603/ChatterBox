using System.ComponentModel.DataAnnotations.Schema;

namespace ChatterBox.Models
{
    public class UserFollow
    {
        public string FollowerId { get; set; }
        public User Follower { get; set; }

        public string FollowedUserId { get; set; }
        public User FollowedUser { get; set; }
    }
}