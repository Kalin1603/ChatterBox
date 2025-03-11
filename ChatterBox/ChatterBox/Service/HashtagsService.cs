using ChatterBox.Data;
using ChatterBox.Helpers;
using ChatterBox.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatterBox.Services
{
    public class HashtagsService
    {
        private readonly ApplicationDbContext _context;
        public HashtagsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcessHashtagsForNewPostAsync(string content)
        {
            var hashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in hashtags)
            {
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == hashtag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count += 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    _context.Hashtags.Update(hashtagDb);
                }
                else
                {
                    var newHashtag = new Hashtag()
                    {
                        Name = hashtag,
                        Count = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };
                    await _context.Hashtags.AddAsync(newHashtag);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task ProcessHashtagsForRemovedPostAsync(string content)
        {
            var hashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in hashtags)
            {
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == hashtag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count -= 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    if (hashtagDb.Count <= 0)
                    {
                        _context.Hashtags.Remove(hashtagDb);
                    }
                    else
                    {
                        _context.Hashtags.Update(hashtagDb);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}