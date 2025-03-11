using ChatterBox.Data;
using ChatterBox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatterBox.ViewComponents
{
    public class HashtagsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public HashtagsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            var topHashtags = await _context.Hashtags
                .Where(h => h.DateCreated >= oneWeekAgo)
                .OrderByDescending(h => h.Count)
                .Take(3)
                .ToListAsync();

            return View(topHashtags);
        }
    }
}