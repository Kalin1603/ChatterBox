using System.Diagnostics;
using System.Security.Claims;
using ChatterBox.Data;
using ChatterBox.Models;
using ChatterBox.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatterBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env; 
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                                      .Include(p => p.User)
                                      .OrderByDescending(p => p.DateCreated)
                                      .ToListAsync();

            var viewModel = new HomeViewModel
            {
                Posts = posts.Where(p => !string.IsNullOrEmpty(p.Content)).ToList(),
                Stories = posts.Where(p => string.IsNullOrEmpty(p.Content) && !string.IsNullOrEmpty(p.ImageURL)).ToList(),
                StatusMessage = (string)TempData["StatusMessage"]
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost(string content, IFormFile uploadContent)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Content is required.";
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

            var post = new Post
            {
                Content = content,
                DateCreated = DateTime.Now,
                User = user
            };

            if (uploadContent != null && uploadContent.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadContent.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadContent.CopyToAsync(fileStream);
                }
                post.ImageURL = "/uploads/posts/" + uniqueFileName;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateStory(IFormFile image)
        {
            ModelState.Remove("Content");

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

            var post = new Post
            {
                Content = string.Empty,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
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

            post.ImageURL = "/uploads/stories/" + uniqueFileName;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
