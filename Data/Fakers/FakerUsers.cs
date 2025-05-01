namespace CesiZen_API.Data.Fakers
{
    using Bogus; 

    public class FakerUsers
    {
        public static void SeedUsers(AppDbContext dbContext, int count = 50)
        {
            var faker = new Faker<User>()
                .RuleFor(u => u.Login, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Role, f => f.PickRandom(new[] { "Admin", "User" }))
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(2))
                .RuleFor(u => u.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(u => u.Password, f => f.Internet.Password());

            var users = faker.Generate(count);

            foreach (var user in users)
            {
                user.SetPassword(user.Password);
            }

            dbContext.Set<User>().AddRange(users);
            dbContext.SaveChanges();
        }
    }
}
