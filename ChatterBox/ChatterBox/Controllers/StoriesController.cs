using ChatterBox.Data;
using ChatterBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatterBox.Controllers
{
    public class StoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StoriesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateStory(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                TempData["Error"] = "Image is required.";
                return RedirectToAction("Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var story = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                User = user
            };

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "stories");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            story.ImageURL = "/uploads/stories/" + uniqueFileName;

            _context.Stories.Add(story);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var story = await _context.Stories.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (story == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (story.User.Id != userId)
            {
                return RedirectToAction("Login", "Account");
            }

            var filePath = Path.Combine(_env.WebRootPath, story.ImageURL?.TrimStart('/') ?? string.Empty);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Stories.Remove(story);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "The story was removed successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
