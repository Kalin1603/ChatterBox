using ChatterBox.Data;
using ChatterBox.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatterBox.ViewComponents
{
    public class ChatUsersViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ChatUsersViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return View("Default", new List<SuggestedUserViewModel>());
            }

            var followingUsers = await _context.UserFollows
                .Where(uf => uf.FollowerId == currentUserId)
                .Select(uf => new SuggestedUserViewModel
                {
                    User = uf.FollowedUser,
                    IsFollowed = true, // Since they are followed, set to true
                })
                .ToListAsync();

            var followersUsers = await _context.UserFollows
                .Where(uf => uf.FollowedUserId == currentUserId)
                .Select(uf => new SuggestedUserViewModel
                {
                    User = uf.Follower,
                    IsFollower = true, // Since they are followers, set to true
                })
                .ToListAsync();

            var combinedUsers = followingUsers.Concat(followersUsers).GroupBy(u => u.User.Id).Select(g => g.First()).ToList();

            return View("Default", combinedUsers);
        }
    }
}
