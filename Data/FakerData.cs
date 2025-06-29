using Bogus;
using CesiZen_API.Helper;
using CesiZen_API.Models;

namespace CesiZen_API.Data
{
    public class FakerData
    {
        public static void SeedAllData(AppDbContext dbContext)
        {
            var users = GenerateUsers(dbContext);
            var categories = GenerateCategories(dbContext, users);
            var activites = GenerateActivites(dbContext, users, categories);
            var menus = GenerateMenus(dbContext, users);
            var pages = GeneratePages(dbContext, users, menus);
            GenerateSaveActivities(dbContext, users, activites);

            dbContext.SaveChanges();
        }

        private static List<User> GenerateUsers(AppDbContext dbContext)
        {
            var admin = new User
            {
                Login = "admin",
                Email = "admin@example.com",
                Role = Constants.ROLE_ADMIN,
                CreatedAt = DateTime.Now.AddMonths(-2),
                UpdatedAt = DateTime.Now,
                Disabled = false,
                Banned = false
            };
            admin.SetPassword("popo");

            var user = new User
            {
                Login = "user",
                Email = "user@example.com",
                Role = Constants.ROLE_USER,
                CreatedAt = DateTime.Now.AddMonths(-2),
                UpdatedAt = DateTime.Now,
                Disabled = false,
                Banned = false
            };
            user.SetPassword("popo");

            var faker = new Faker<User>("fr")
                .RuleFor(u => u.Login, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Role, f => f.PickRandom(new[] { Constants.ROLE_USER }))
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(2))
                .RuleFor(u => u.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.Disabled, f => false)
                .RuleFor(u => u.Banned, f => false);

            var users = faker.Generate(18);
            users.ForEach(u => u.SetPassword("popo"));

            users.Insert(0, admin);
            users.Insert(1, user);

            dbContext.User.AddRange(users);
            return users;
        }

        private static List<Category> GenerateCategories(AppDbContext dbContext, List<User> users)
        {
            var emojis = new[] { "🧘", "🏃", "🎧", "🎨", "📚", "☀️", "💤", "🔥", "🌙" };
            var durations = new[]
            {
                "3 à 5 minutes", "5 à 10 minutes", "10 à 15 minutes",
                "15 à 20 minutes", "1 à 2 minutes", "2 à 4 minutes",
                "7 à 12 minutes", "5 à 7 minutes", "20 à 30 minutes"
            };

            var categories = new List<Category>();
            int index = 0;
            foreach (var kvp in Constants.FAKE_CATEGORY)
            {
                categories.Add(new Category
                {
                    Name = kvp.Value,
                    Emoji = emojis[index % emojis.Length],
                    Duration = durations[index % durations.Length],
                    Visibility = index % 3 != 0, 
                    CreatedAt = DateTime.Now.AddDays(-10 + index),
                    User = users[(index + 2) % users.Count]
                });
                index++;
            }

            dbContext.Category.AddRange(categories);
            return categories;
        }

        private static List<Activite> GenerateActivites(AppDbContext dbContext, List<User> users, List<Category> categories)
        {
            var visibleCategories = categories.Where(c => c.Visibility == Constants.STATUS_ACTIVE).ToList();
            var faker = new Faker<Activite>("fr")
                .RuleFor(a => a.Title, f => f.Lorem.Sentence(3).Truncate(30))
                .RuleFor(a => a.Duree, f => f.Random.Int(3, 20))
                .RuleFor(a => a.Description, f => f.Lorem.Paragraph().Truncate(255))
                .RuleFor(a => a.TypeActitvity, f => f.PickRandom(Constants.ACTIVITY_TYPES.Values.ToList()))
                .RuleFor(a => a.Url, f => "/medias/" + f.PickRandom(Constants.FAKE_MEDIA))
                .RuleFor(a => a.ImagePresentation, f => "/images/" + f.PickRandom(Constants.FAKE_IMAGE))
                .RuleFor(a => a.Status, f => f.PickRandom(new[] { Constants.STATUS_ACTIVE, Constants.STATUS_INACTIVE }))
                .RuleFor(a => a.CreatedAt, f => f.Date.Past())
                .RuleFor(a => a.Category, f => f.PickRandom(visibleCategories))
                .RuleFor(a => a.User, f => f.PickRandom(users));

            var activites = faker.Generate(20);
            dbContext.Activite.AddRange(activites);
            return activites;
        }

        private static List<Menu> GenerateMenus(AppDbContext dbContext, List<User> users)
        {
            var faker = new Faker<Menu>("fr")
                .RuleFor(a => a.Title, f => f.Lorem.Sentence(3).Truncate(30))
                .RuleFor(m => m.Status, f => Constants.STATUS_ACTIVE)
                .RuleFor(m => m.CreatedAt, f => f.Date.Past())
                .RuleFor(m => m.User, f => f.PickRandom(users))
                .RuleFor(m => m.Parent, f => null);

            var menus = faker.Generate(5);
            dbContext.Menu.AddRange(menus);
            return menus;
        }

        private static List<Page> GeneratePages(AppDbContext dbContext, List<User> users, List<Menu> menus)
        {
            var faker = new Faker<Page>("fr")
                .RuleFor(p => p.Title, f => f.Company.CatchPhrase())
                .RuleFor(p => p.Content, f => $@"
                    <h1>{f.Commerce.ProductName()}</h1>
                    <p>{f.Lorem.Paragraph(2)}</p>
                    <ul>
                        <li>{f.Lorem.Sentence()}</li>
                        <li>{f.Lorem.Sentence()}</li>
                    </ul>
                    <p><strong>{f.Lorem.Sentence()}</strong></p>
                ")
                .RuleFor(p => p.Visibility, f => f.Random.Bool())
                .RuleFor(p => p.Menu, f => f.PickRandom(menus))
                .RuleFor(p => p.User, f => f.PickRandom(users));

            var pages = faker.Generate(10);
            dbContext.Page.AddRange(pages);
            return pages;
        }

        private static void GenerateSaveActivities(AppDbContext dbContext, List<User> users, List<Activite> activites)
        {
            var saves = new List<SaveActivity>();
            var random = new Random();
            int count = 0;

            foreach (var user in users)
            {
                var userActivities = activites
                    .Where(a => a.Status == Constants.STATUS_ACTIVE)
                    .OrderBy(_ => random.Next())
                    .Take(random.Next(1, 4));

                foreach (var activity in userActivities)
                {
                    saves.Add(new SaveActivity
                    {
                        User = user,
                        Activite = activity,
                        IsFavorite = random.NextDouble() > 0.5,
                        IsToLater = random.NextDouble() > 0.5
                    });
                    count++;
                }
            }

            dbContext.SaveActivity.AddRange(saves);
        }
    }

    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
