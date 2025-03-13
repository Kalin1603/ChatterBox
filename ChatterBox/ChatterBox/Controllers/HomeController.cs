using System.Diagnostics;
using System.Security.Claims;
using ChatterBox.Data;
using ChatterBox.Models;
using ChatterBox.Services;
using ChatterBox.ViewModels.Home;
using ChatterBox.ViewModels.UserViewModels;
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
        private readonly HashtagsService _hashtagsService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env, HashtagsService hashtagsService)
        {
            _logger = logger;
            _context = context;
            _env = env;
            _hashtagsService = hashtagsService;
        }

        public async Task<IActionResult> Index(string hashtag)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<Post> query = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes).ThenInclude(l => l.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Favorites).ThenInclude(f => f.User)
                .OrderByDescending(p => p.DateCreated);

            var suggestedUsers = await _context.Users
                .Where(u => u.Id != currentUserId) 
                .Take(5)
                .Select(u => new SuggestedUserViewModel
                {
                    User = u,
                    IsFollowed = _context.UserFollows
                        .Any(uf => uf.FollowerId == currentUserId && uf.FollowedUserId == u.Id)
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(hashtag))
            {
                query = query.Where(p => p.Content.Contains(hashtag));
            }

            var posts = await query.ToListAsync();

            var viewModel = new HomeViewModel
            {
                Posts = posts.Where(p => !string.IsNullOrEmpty(p.Content)).ToList(),
                StatusMessage = (string)TempData["StatusMessage"],
                PeopleYouMayKnow = suggestedUsers
            };

            return View(viewModel);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                TempData["Error"] = "Please enter a search term.";
                return RedirectToAction("Index");
            }

            var posts = await _context.Posts
                            .Include(p => p.User)
                            .Where(p => p.Content.Contains(query))
                            .OrderByDescending(p => p.DateCreated)
                            .ToListAsync();

            var users = await _context.Users
                .Where(u => (u.FullName != null && u.FullName.Contains(query)) || (u.UserName != null && u.UserName.Contains(query)))
                .ToListAsync();

            var viewModel = new SearchViewModel
            {
                Query = query,
                Posts = posts,
                Users = users
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
                return RedirectToPage("Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToPage("Login");
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

            TempData["StatusMessage"] = "The post was created successfully.";

            await _hashtagsService.ProcessHashtagsForNewPostAsync(content);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLike(PostLikeViewModel postLikeViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postLikeViewModel.PostId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like
                {
                    PostId = postLikeViewModel.PostId,
                    UserId = userId
                };
                await _context.Likes.AddAsync(newLike);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment(PostCommentViewModel postComment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToPage("Login");
            }

            var newComment = new Comment
            {
                User = user,
                PostId = postComment.PostId,
                Content = postComment.Content,
                DateCreated = DateTime.Now,
                DateUpdate = DateTime.Now
            };

            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveComment(RemoveCommentViewModel removeComment)
        {
            var comment = await _context.Comments.FindAsync(removeComment.CommentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReportPost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                TempData["Error"] = "The post was not found.";
                return RedirectToAction("Index");
            }

            var report = new Report
            {
                PostId = postId,
                UserId = userId,
                DateCreated = DateTime.Now
            };

            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "The report was sent successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetPostAsPrivate(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                TempData["Error"] = "The post was not found.";
                return RedirectToAction("Index");
            }

            if (post.UserId != userId)
            {
                TempData["Error"] = "You have no rights to delete this post.";
                return RedirectToAction("Index");
            }

            post.IsPrivate = true;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "The post was set to private.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetPostAsPublic(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                TempData["Error"] = "The post was not found.";
                return RedirectToAction("Index");
            }

            if (post.UserId != userId)
            {
                TempData["Error"] = "You have no rights to delete this post.";
                return RedirectToAction("Index");
            }

            post.IsPrivate = false;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "The post was set to public.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                TempData["Error"] = "Post not found.";
                return RedirectToAction("Index");
            }

            if (post.UserId != userId)
            {
                TempData["Error"] = "You have no rights to delete this post.";
                return RedirectToAction("Index");
            }

            await _hashtagsService.ProcessHashtagsForRemovedPostAsync(post.Content);

            var filePath = Path.Combine(_env.WebRootPath, post.ImageURL?.TrimStart('/') ?? string.Empty);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "The post was removed successfully.";
            return RedirectToAction("Index");
        }
    }
}
