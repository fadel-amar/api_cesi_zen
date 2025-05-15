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
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Login, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Role, f => f.PickRandom(new[] { Constants.ROLE_USER, Constants.ROLE_ADMIN }))
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(2))
                .RuleFor(u => u.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.Disabled, f => false)
                .RuleFor(u => u.Banned, f => false);

            var users = userFaker.Generate(20);
            users.ForEach(u => u.SetPassword(u.Password));
            dbContext.User.AddRange(users);
            return users;
        }

        private static List<Category> GenerateCategories(AppDbContext dbContext, List<User> users)
        {
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.PickRandom(new[] { "Esprit", "Corps", "Voyager" }))
                .RuleFor(c => c.Visibility, f => f.Random.Bool())
                .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                .RuleFor(c => c.User, f => f.PickRandom(users));

            var categories = categoryFaker.Generate(10);
            dbContext.Category.AddRange(categories);
            return categories;
        }

        private static List<Activite> GenerateActivites(AppDbContext dbContext, List<User> users, List<Category> categories)
        {
            var activiteFaker = new Faker<Activite>()
                .RuleFor(a => a.Title, f => f.Lorem.Sentence(3))
                .RuleFor(a => a.Duree, f => f.Date.Timespan(TimeSpan.FromMinutes(90)))
                .RuleFor(a => a.Description, f => f.Lorem.Sentence(10))
                .RuleFor(a => a.TypeActitvity, f => f.PickRandom(new[] { "Yoga", "Meditation", "Course", "Détente" }))
                .RuleFor(a => a.url, f => f.Internet.Url())
                .RuleFor(a => a.Status, f => f.PickRandom(new[] {Constants.STATUS_ACTIVE, Constants.STATUS_INACTIVE }))
                .RuleFor(a => a.CreatedAt, f => f.Date.Past())
                .RuleFor(a => a.Category, f => f.PickRandom(categories));

            var activites = activiteFaker.Generate(20);
            dbContext.Activite.AddRange(activites);
            return activites;
        }

        private static List<Menu> GenerateMenus(AppDbContext dbContext, List<User> users)
        {
            var menuFaker = new Faker<Menu>()
                .RuleFor(m => m.Title, f => f.Lorem.Word())
                .RuleFor(m => m.Status, f => f.PickRandom(new[] { Constants.STATUS_ACTIVE, Constants.STATUS_INACTIVE }))
                .RuleFor(m => m.DateCreation, f => f.Date.Past())
                .RuleFor(m => m.User, f => f.PickRandom(users))
                .RuleFor(m => m.Parent, f => null); 

            var menus = menuFaker.Generate(5);
            dbContext.Menu.AddRange(menus);
            return menus;
        }

        private static List<Page> GeneratePages(AppDbContext dbContext, List<User> users, List<Menu> menus)
        {
            var pageFaker = new Faker<Page>()
                .RuleFor(p => p.Title, f => f.Lorem.Word())
                .RuleFor(p => p.Content, f => f.Lorem.Text())
                .RuleFor(p => p.Visibility, f => f.Random.Bool())
                .RuleFor(p => p.Menu, f => f.PickRandom(menus))
                .RuleFor(p => p.User, f => f.PickRandom(users));

            var pages = pageFaker.Generate(15);
            dbContext.Page.AddRange(pages);
            return pages;
        }

        private static void GenerateSaveActivities(AppDbContext dbContext, List<User> users, List<Activite> activites)
        {
            var saves = new List<SaveActivity>();
            var random = new Random();
            int limit = 15;
            int count = 0;

            foreach (var user in users)
            {
                if (count >= limit) break;

                var nbActivities = random.Next(1, 4);
                var randomActivities = activites.OrderBy(_ => random.Next()).Take(nbActivities);

                foreach (var activite in randomActivities)
                {
                    if (count >= limit) break;

                    saves.Add(new SaveActivity
                    {
                        User = user,
                        Activite = activite,
                        isFavorite = random.NextDouble() > 0.5,
                        isToLater = random.NextDouble() > 0.5
                    });
                    count++;
                }
            }

            dbContext.SaveActivity.AddRange(saves);
        }

    }
}
