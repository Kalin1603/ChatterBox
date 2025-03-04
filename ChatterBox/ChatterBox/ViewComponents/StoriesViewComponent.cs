using ChatterBox.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatterBox.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public StoriesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stories = await _context.Stories
                .Where(s => s.DateCreated >= DateTime.UtcNow.AddHours(-24))
                .Include(s => s.User)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();

            return View(stories);
        }
    }
}