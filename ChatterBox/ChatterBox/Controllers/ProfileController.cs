using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatterBox.Data;
using ChatterBox.ViewModels.Profile;
using ChatterBox.Models;
using System.Security.Claims;
using ChatterBox.ViewModels.UserViewModels;
using ChatterBox.Enums;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using ChatterBox.ViewModels.Home;

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
                StatusMessage = (string)TempData["StatusMessage"],
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
                    .AnyAsync(uf => uf.FollowerId == currentUserId && uf.FollowedUserId == id),
                IsFollowPending = await _context.Notifications.AnyAsync(n =>
                    n.SenderId == currentUserId &&
                    n.ReceiverId == id &&
                    n.Type == NotificationType.FollowRequest
                )
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

            var sender = await _context.Users.FindAsync(currentUserId);

            var existingNotification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.SenderId == currentUserId
                                          && n.ReceiverId == followedUserId
                                          && n.Type == NotificationType.FollowRequest);

            if (existingNotification == null)
            {
                var notification = new Notification
                {
                    SenderId = currentUserId,
                    ReceiverId = followedUserId,
                    Type = NotificationType.FollowRequest,
                    Message = $"{sender?.FullName} wants to follow you.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                TempData["StatusMessage"] = "The request is sent!";
            }
            else
            {
                TempData["StatusMessage"] = "The request is already sent!";
            }

            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow(string unfollowUserId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(unfollowUserId))
            {
                return BadRequest();
            }

            var followingUser = await _context.Users.FindAsync(unfollowUserId);

            var followRelation = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowedUserId == unfollowUserId);

            if (followRelation != null)
            {
                _context.UserFollows.Remove(followRelation);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"You have unfollowed {followingUser?.FullName}!";
            }
            else
            {
                TempData["StatusMessage"] = "You are not following this user.";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFollower(string followerId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(followerId))
            {
                return BadRequest();
            }

            var followerRelation = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowedUserId == currentUserId);

            if (followerRelation != null)
            {
                _context.UserFollows.Remove(followerRelation);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"You have removed the follower!";
            }
            else
            {
                TempData["StatusMessage"] = "This follower does not exist.";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFollowRequest(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if (notification == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sender = await _context.Users.FindAsync(notification.SenderId);

            if (notification.ReceiverId != currentUserId)
            {
                return Unauthorized();
            }

            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == notification.SenderId && uf.FollowedUserId == currentUserId);

            if (existingFollow == null)
            {
                var userFollow = new UserFollow
                {
                    FollowerId = notification.SenderId,
                    FollowedUserId = currentUserId
                };
                _context.UserFollows.Add(userFollow);
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"{sender?.FullName} is now following you!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> RejectFollowRequest(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if (notification == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (notification.ReceiverId != currentUserId)
            {
                return Unauthorized();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "The request is rejected!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> SuggestedFriends()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var suggestedUsers = await _context.Users
                .Where(u =>
                    u.Id != currentUserId &&
                    !_context.UserFollows.Any(uf =>
                        uf.FollowerId == currentUserId && uf.FollowedUserId == u.Id
                    )
                )
                .Select(u => new SuggestedUserViewModel
                {
                    User = u,
                    IsFollowed = false,
                    IsFollowPending = _context.Notifications.Any(n =>
                        n.SenderId == currentUserId &&
                        n.ReceiverId == u.Id &&
                        n.Type == NotificationType.FollowRequest
                    )
                })
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                PeopleYouMayKnow = suggestedUsers,
                StatusMessage = (string)TempData["StatusMessage"]
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Chat(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var isFollowing = await _context.UserFollows.AnyAsync(uf =>
                (uf.FollowerId == currentUserId && uf.FollowedUserId == userId) ||
                (uf.FollowerId == userId && uf.FollowedUserId == currentUserId));

            if (!isFollowing) return BadRequest("You can only chat with users you follow or who follow you.");

            var chat = await _context.Chats.FirstOrDefaultAsync(c =>
                (c.InitiatorId == currentUserId && c.RecipientId == userId) ||
                (c.InitiatorId == userId && c.RecipientId == currentUserId));

            if (chat == null)
            {
                chat = new Chat { InitiatorId = currentUserId, RecipientId = userId };
                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
            }

            var messages = await _context.Messages
                .Where(m => m.ChatId == chat.Id)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.ChatId = chat.Id;
            return View("Chat", messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string content)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId)) return Unauthorized();

            if (string.IsNullOrEmpty(content)) return BadRequest("Message content cannot be empty.");

            try
            {
                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = senderId,
                    Content = content,
                    SentAt = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to send message.");
            }
        }
    }
}