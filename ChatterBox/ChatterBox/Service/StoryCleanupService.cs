using ChatterBox.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class StoryCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); 

    public StoryCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var expirationDate = DateTime.UtcNow.AddHours(-24);

                var expiredStories = context.Stories.Where(s => s.DateCreated < expirationDate);

                if (expiredStories.Any())
                {
                    context.Stories.RemoveRange(expiredStories);
                    await context.SaveChangesAsync(stoppingToken);
                }
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}