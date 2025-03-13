using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatterBox.Data;
using ChatterBox.ViewModels.Profile;
using ChatterBox.Models;
using System.Security.Claims;
using ChatterBox.ViewModels.UserViewModels;

namespace ChatterBox.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(u => u.Followers)
                    .ThenInclude(uf => uf.Follower)
                .Include(u => u.Followings)
                    .ThenInclude(uf => uf.FollowedUser)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var suggestedUsers = await _context.Users
                .Where(u =>
                    u.Id != currentUserId &&
                    u.Id != id &&
                    !_context.UserFollows.Any(uf =>
                        uf.FollowerId == currentUserId &&
                        uf.FollowedUserId == u.Id
                    )
                )
                .Take(5)
                .Select(u => new SuggestedUserViewModel
                {
                    User = u,
                    IsFollowed = false 
                })
                .ToListAsync();

            var viewModel = new ProfileViewModel
            {
                User = user,
                Posts = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Comments).ThenInclude(c => c.User)
                    .Where(p => p.UserId == id)
                    .OrderByDescending(p => p.DateCreated)
                    .ToListAsync(),
                PeopleYouMayKnow = suggestedUsers,
                Followers = user.Followers
                    .Select(uf => new SuggestedUserViewModel
                    {
                        User = uf.Follower,
                        IsFollowed = _context.UserFollows
                            .Any(f => f.FollowerId == currentUserId && f.FollowedUserId == uf.Follower.Id)
                    })
                    .ToList(),
                Followings = user.Followings
                    .Select(uf => new SuggestedUserViewModel
                    {
                        User = uf.FollowedUser,
                        IsFollowed = _context.UserFollows
                            .Any(f => f.FollowerId == currentUserId && f.FollowedUserId == uf.FollowedUser.Id)
                    })
                    .ToList(),
                IsFollowed = await _context.UserFollows
                    .AnyAsync(uf => uf.FollowerId == currentUserId && uf.FollowedUserId == id)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Follow(string followedUserId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(followedUserId))
            {
                return BadRequest();
            }

            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowedUserId == followedUserId);

            if (existingFollow == null)
            {
                var newFollow = new UserFollow
                {
                    FollowerId = currentUserId,
                    FollowedUserId = followedUserId
                };
                _context.UserFollows.Add(newFollow);
                TempData["StatusMessage"] = "You successfuly followed the user!";
            }
            else
            {
                _context.UserFollows.Remove(existingFollow);
                TempData["StatusMessage"] = "You successfuly unfollowed the user!";
                return RedirectToAction("Index", "Home");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Profile", new { id = currentUserId });
        }
    }
}