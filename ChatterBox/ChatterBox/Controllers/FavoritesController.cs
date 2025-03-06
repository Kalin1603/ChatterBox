using ChatterBox.Data;
using ChatterBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatterBox.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favPosts = await _context.Posts
                .Include(f => f.Reports)
                .Include(f => f.User)
                .Include(f => f.Comments)
                    .ThenInclude(c => c.User)
                .Include(f => f.Likes)
                .Include(f => f.Favorites)
                .Where(n => n.Favorites.Any(f => f.UserId == currentUserId) &&
                            !n.IsDeleted &&
                            n.Reports.Count < 5)
                .OrderByDescending(f => f.DateCreated)
                .ToListAsync();

            return View(favPosts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPostToFavorite(int postId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favorite = await _context.Favorites
                .Where(l => l.PostId == postId && l.UserId == currentUserId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }

            else
            {
                var favoritePost = new Favorite
                {
                    PostId = postId,
                    UserId = currentUserId,
                    DateCreated = DateTime.Now
                };
                _context.Favorites.Add(favoritePost);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
