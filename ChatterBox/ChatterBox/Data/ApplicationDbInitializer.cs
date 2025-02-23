using ChatterBox.Models;

namespace ChatterBox.Data
{
    public class ApplicationDbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any() && !context.Posts.Any())
            {
                var user = new User()
                {
                    ProfilePictureURL = "https://game-tournaments.com/media/news/n37757.jpeg",
                    FullName = "Kalin Todorov",
                    UserName = "forsaken",
                    Email = "kalintodorov03@gmail.com",
                    Country = "Bulgaria",
                    City = "Sofia",
                    State = "Sofia",
                    Address = "bul. Vitosha 1",
                    Zip = "1000",
                    DateOfBirth = new DateTime(1999, 2, 12)
                };
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var postWithoutImage = new Post()
                {
                    Content = "Thsis is going to be our first post which is being loaded from the database and it has been created using our test user.",
                    ImageURL = null,
                    NumberOfReports = 0,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,

                    UserId = user.Id
                };

                var postWithImage = new Post()
                {
                    Content = "This is going to be our second post which is being loaded from the database and it has been created using our test user. This post has an image.",
                    ImageURL = "https://image-proxy.bo3.gg/uploads/news/24900/title_image/webp-9b0db568d73a97beedabd6d609f2f855.webp.webp?w=960&h=480",
                    NumberOfReports = 0,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,

                    UserId = user.Id
                };
                await context.Posts.AddRangeAsync(postWithoutImage, postWithImage);
                await context.SaveChangesAsync();
            }
        }
    }
}
