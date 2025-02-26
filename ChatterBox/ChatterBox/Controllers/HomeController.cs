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
                Posts = posts,
                StatusMessage = (string)TempData["StatusMessage"]
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePost(string content, IFormFile uploadContent)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Content is required.";
                return RedirectToAction("Index");
            }

            // Вземане на ID-то на текущия логнат потребител
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Извличане на потребителя от базата данни
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            // Създаване на нов обект Post
            var post = new Post
            {
                Content = content,
                DateCreated = DateTime.Now,
                User = user
            };

            // Ако има качено изображение, запазваме го в папката wwwroot/uploads
            if (uploadContent != null && uploadContent.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
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
                // Запазваме относителния URL на изображението
                post.ImageURL = "/uploads/" + uniqueFileName;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
