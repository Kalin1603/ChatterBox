using ChatterBox.Data;
using ChatterBox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatterBox.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public NotificationsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return View("Default", new List<Notification>());
            }

            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == currentUserId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View("Default", notifications);
        }
    }
}
