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
                    .ThenInclude(uf => uf.Follower) // Зареждане на Follower за всеки UserFollow
                .Include(u => u.Followings)
                    .ThenInclude(uf => uf.FollowedUser) // Зареждане на FollowedUser за всеки UserFollow
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            // Списък с ID-та на всички последователи и последвани
            var followerIds = user.Followers.Select(uf => uf.Follower.Id).ToList();
            var followingIds = user.Followings.Select(uf => uf.FollowedUser.Id).ToList();

            // Проверка кои от тях currentUserId следва
            var currentUserFollowedIds = await _context.UserFollows
                .Where(uf => uf.FollowerId == currentUserId &&
                       (followerIds.Contains(uf.FollowedUserId) || followingIds.Contains(uf.FollowedUserId)))
                .Select(uf => uf.FollowedUserId)
                .ToListAsync();

            // Попълване на Followers и Followings
            var viewModel = new ProfileViewModel
            {
                User = user,
                Posts = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Comments).ThenInclude(c => c.User)
                    .Where(p => p.UserId == id)
                    .OrderByDescending(p => p.DateCreated)
                    .ToListAsync(),
                Followers = user.Followers
                    .Select(uf => new SuggestedUserViewModel
                    {
                        User = uf.Follower,
                        IsFollowed = currentUserFollowedIds.Contains(uf.Follower.Id)
                    })
                    .ToList(),
                Followings = user.Followings
                    .Select(uf => new SuggestedUserViewModel
                    {
                        User = uf.FollowedUser,
                        IsFollowed = currentUserFollowedIds.Contains(uf.FollowedUser.Id)
                    })
                    .ToList(),
                // ... останалите свойства
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

            // Проверете дали вече съществува follow връзка
            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowedUserId == followedUserId);

            if (existingFollow == null)
            {
                // Създаване на нова follow връзка
                var newFollow = new UserFollow
                {
                    FollowerId = currentUserId,
                    FollowedUserId = followedUserId
                };
                _context.UserFollows.Add(newFollow);
            }
            else
            {
                // Ако връзката съществува, изтриваме я (Unfollow)
                _context.UserFollows.Remove(existingFollow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = followedUserId });
        }
    }
}