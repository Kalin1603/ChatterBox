using ChatterBox.Data;
using ChatterBox.Models;
using ChatterBox.ViewModels.Home;
using ChatterBox.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            return View("Default", new List<NotificationViewModel>());
        }

        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.ReceiverId == currentUserId && !n.IsRead)
            .Include(n => n.Sender)
            .Include(n => n.Chat)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationViewModel
            {
                Id = n.Id,
                Message = n.Message,
                Type = n.Type,
                ChatId = n.ChatId ?? 0,
                SenderProfile = new ProfileViewModel { User = n.Sender! }
            })
            .ToListAsync();

        return View("Default", notifications);
    }
}