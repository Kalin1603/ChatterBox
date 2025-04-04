using ChatterBox.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatterBox.Data
{
    public static class ApplicationDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Create a scope to retrieve scoped services.
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Apply any pending migrations.
            await context.Database.MigrateAsync();

            // Define sample users with profile images and confirmed emails.
            var sampleUsers = new List<User>
            {
                new User {
                    UserName = "alice@example.com",
                    Email = "alice@example.com",
                    FullName = "Alice Johnson",
                    Country = "USA",
                    City = "New York",
                    State = "NY",
                    Address = "600 Banana Rd", 
                    Zip = "M5H 2N2",
                    DateOfBirth = new DateTime(1990, 5, 12),
                    RegistrationDate = DateTime.UtcNow,
                    ProfilePictureURL = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    EmailConfirmed = true
                },
                new User {
                    UserName = "bob@example.com",
                    Email = "bob@example.com",
                    FullName = "Bob Smith",
                    Country = "USA",
                    City = "Los Angeles",
                    State = "CA",
                    Address = "865 Roboc Rd", 
                    Zip = "M5H 2N2",
                    DateOfBirth = new DateTime(1988, 8, 23),
                    RegistrationDate = DateTime.UtcNow,
                    ProfilePictureURL = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    EmailConfirmed = true
                },
                new User {
                    UserName = "charlie@example.com",
                    Email = "charlie@example.com",
                    FullName = "Charlie Davis",
                    Country = "Canada",
                    City = "Toronto",
                    State = "ON",
                    Address = "900 Food Rd", 
                    Zip = "M5H 2N2",
                    DateOfBirth = new DateTime(1992, 3, 15),
                    RegistrationDate = DateTime.UtcNow,
                    ProfilePictureURL = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    EmailConfirmed = true
                },
                new User {
                    UserName = "diana@example.com",
                    Email = "diana@example.com",
                    FullName = "Diana Ross",
                    Country = "UK",
                    City = "London",
                    State = "London",
                    Address = "800 Apple Rd", 
                    Zip = "M5H 2N2",
                    DateOfBirth = new DateTime(1985, 11, 30),
                    RegistrationDate = DateTime.UtcNow,
                    ProfilePictureURL = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    EmailConfirmed = true
                },
                new User {
                    UserName = "edward@example.com",
                    Email = "edward@example.com",
                    FullName = "Edward Norton",
                    Country = "USA",
                    City = "Chicago",
                    State = "IL",
                    Address = "789 Maple Rd", 
                    Zip = "M5H 2N2",
                    DateOfBirth = new DateTime(1991, 7, 7),
                    RegistrationDate = DateTime.UtcNow,
                    ProfilePictureURL = "https://images.unsplash.com/photo-1525134479668-1bee5c7c6845?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    EmailConfirmed = true
                }
            };

            // List to hold the sample users (newly created or existing) to seed posts and stories.
            var seededUsers = new List<User>();

            // Loop through each sample user.
            foreach (var sampleUser in sampleUsers)
            {
                // Check if the user already exists (by email).
                var existingUser = await userManager.FindByEmailAsync(sampleUser.Email);
                if (existingUser == null)
                {
                    // If not found, create the user.
                    var result = await userManager.CreateAsync(sampleUser, "Password123!");
                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to create sample user: " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                    seededUsers.Add(sampleUser);
                }
                else
                {
                    seededUsers.Add(existingUser);
                }
            }

            // Define different posts and stories for each sample user.
            var samplePosts = new Dictionary<string, (string Content, string ImageURL)>
            {
                { "alice@example.com", ("Alice's adventures in New York!", "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7a/View_of_Empire_State_Building_from_Rockefeller_Center_New_York_City_dllu_%28cropped%29.jpg/800px-View_of_Empire_State_Building_from_Rockefeller_Center_New_York_City_dllu_%28cropped%29.jpg") },
                { "bob@example.com", ("Bob's sunny day in Los Angeles.", "https://drupal-prod.visitcalifornia.com/sites/default/files/styles/fluid_1920/public/2020-06/VC_PlacesToVisit_LosAngelesCounty_RF_1170794243.jpg.webp?itok=46pJYz8v") },
                { "charlie@example.com", ("Charlie exploring the streets of Toronto.", "https://images.prismic.io/bounce/Z0ShZa8jQArT1SSJ_is-toronto-safe.jpg?auto=format%2Ccompress&w=1466&fit=crop&ar=3%3A2") },
                { "diana@example.com", ("Diana enjoying London life.", "https://www.thecityofldn.com/wp-content/uploads/2023/05/jamiesmithphoto-2162-Tower-Bridge-with-City-behind-reduced-2000x1334.jpg") },
                { "edward@example.com", ("Edward's Chicago adventures.", "https://cdn.britannica.com/59/94459-050-DBA42467/Skyline-Chicago.jpg") }
            };

            var sampleStories = new Dictionary<string, (string StoryText, string ImageURL)>
            {
                { "alice@example.com", ("Check out this cool view!", "https://media.cntraveler.com/photos/675f30d1796d888fdbf7c595/1:1/w_2001,h_2001,c_limit/Bark-Beach-miami_GettyImages-1540357059.jpg") },
                { "bob@example.com", ("Amazing sunset at the beach.", "https://www.tiffanymccauley.com/wp-content/uploads/2023/11/beach-.jpg") },
                { "charlie@example.com", ("Street art in Toronto!", "https://cdn.europosters.eu/image/750/posters/beach-sunset-i102003.jpg") },
                { "diana@example.com", ("A day in London.", "https://www.longbeachresort.bg/wp-content/uploads/2024/03/Untitled-5-768x576.jpg") },
                { "edward@example.com", ("Exploring Chicago downtown.", "https://www.travelandleisure.com/thmb/f_rDORCl4bSfbktUu2Gm-saHolI=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/mauna-kea-besthawaii0216-771df2153a094b389fec930a0adac8e5.jpg") }
            };

            // For each seeded user, add a post and story if they don't already have one.
            foreach (var user in seededUsers)
            {
                // Check if a post for this sample user already exists.
                if (!context.Posts.Any(p => p.UserId == user.Id))
                {
                    if (samplePosts.TryGetValue(user.Email, out var postData) &&
                        sampleStories.TryGetValue(user.Email, out var storyData))
                    {
                        // Create a sample post with unique content and image.
                        var post = new Post
                        {
                            Content = postData.Content,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow,
                            IsDeleted = false,
                            IsPrivate = false,
                            UserId = user.Id,
                            ImageURL = postData.ImageURL
                        };
                        context.Posts.Add(post);

                        // Create a sample story with unique image.
                        var story = new Story
                        {
                            ImageURL = storyData.ImageURL,
                            DateCreated = DateTime.UtcNow,
                            IsDeleted = false,
                            UserId = user.Id
                        };
                        context.Stories.Add(story);

                        // Optionally, add a comment and a like on the post from the same user.
                        var comment = new Comment
                        {
                            Content = $"Comment by {user.FullName} on their own post.",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Post = post,
                            UserId = user.Id
                        };
                        context.Comments.Add(comment);

                        var like = new Like
                        {
                            Post = post,
                            UserId = user.Id
                        };
                        context.Likes.Add(like);
                    }
                }
            }

            // Save all seeded data to the database.
            await context.SaveChangesAsync();
        }
    }
}
