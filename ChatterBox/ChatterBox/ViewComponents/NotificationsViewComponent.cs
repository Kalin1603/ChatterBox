using ChatterBox.Data;
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
            .Where(n => n.ReceiverId == currentUserId)
            .Include(n => n.Sender)  
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        // Map Notifications to NotificationViewModels
        var viewModels = notifications.Select(n => new NotificationViewModel
        {
            Id = n.Id,
            Message = n.Message,
            Type = n.Type,
            SenderProfile = new ProfileViewModel
            {
                User = n.Sender  
            }
        }).ToList();

        return View("Default", viewModels);  
    }
}