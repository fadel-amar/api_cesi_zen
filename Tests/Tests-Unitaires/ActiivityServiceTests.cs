using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CesiZen_API.DTO;
using CesiZen_API.Helper;
using CesiZen_API.Helper.Exceptions;
using CesiZen_API.Models;
using CesiZen_API.Services;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Tests.Services
{
    public class ActivityServiceTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;
        private readonly Mock<ICategoryService> _mockCategoryService;

        public ActivityServiceTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(_options);
            _context.Database.EnsureCreated();

            _mockCategoryService = new Mock<ICategoryService>();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private IFormFile CreateMockFormFile(string fileName, string contentType, byte[] content)
        {
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns(fileName);
            file.Setup(f => f.ContentType).Returns(contentType);
            file.Setup(f => f.Length).Returns(content.Length);
            file.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(content));
            return file.Object;
        }

        [Fact]
        public async Task GetAllActivities_ReturnsAllActivities_WhenIsAdmin()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };

            _context.Activite.Add(new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 });
            _context.Activite.Add(new Activite { Id = 2, Title = "Activity2", Description = "Description2", Status = false, Category = category, User = user, TypeActitvity = "Type2", ImagePresentation = "http://example.com/image2.jpg", Url = "http://example.com/url2", Duree = 45 });
            await _context.SaveChangesAsync();

            var result = await service.GetAllActivities(true, null);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllActivities_ReturnsOnlyVisibleActivities_WhenNotAdmin()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };

            _context.Activite.Add(new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 });
            _context.Activite.Add(new Activite { Id = 2, Title = "Activity2", Description = "Description2", Status = false, Category = category, User = user, TypeActitvity = "Type2", ImagePresentation = "http://example.com/image2.jpg", Url = "http://example.com/url2", Duree = 45 });
            await _context.SaveChangesAsync();

            var result = await service.GetAllActivities(false, null);

            Assert.Single(result);
            Assert.Equal("Activity1", result.First().Title);
        }

        [Fact]
        public async Task GetActivityById_ReturnsActivity_WhenActivityExistsAndIsVisible()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };

            _context.Activite.Add(new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 });
            await _context.SaveChangesAsync();

            var result = await service.GetActivityById(1, false);

            Assert.NotNull(result);
            Assert.Equal("Activity1", result.Title);
        }

        [Fact]
        public async Task GetActivityById_ThrowsNotFoundException_WhenActivityDoesNotExist()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetActivityById(999, false));
        }

        [Fact]
        public async Task CreateActivity_AddsActivityToDatabase()
        {
            // Arrange
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };
            _context.Category.Add(category);
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            _mockCategoryService
            .Setup(cs => cs.GetCategoryById(1, true))
            .ReturnsAsync(category);


            var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imageDir))
                Directory.CreateDirectory(imageDir);

            var mediaDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "medias");
            if (!Directory.Exists(mediaDir))
                Directory.CreateDirectory(mediaDir);

            var imagePath = Path.Combine(imageDir, "test.jpg");
            await File.WriteAllBytesAsync(imagePath, new byte[] { 1, 2, 3 }); // fichier fictif

            var dto = new CreateActivityDTO
            {
                Title = "Test Activity",
                Description = "This is a test activity.",
                CategoryId = 1,
                TypeActivitty = 1,
                ImagePresentation = new FormFile(new MemoryStream(new byte[] { 1, 2, 3 }), 0, 3, "file", "test.jpg"),
                Url = new FormFile(new MemoryStream(new byte[] { 1, 2, 3 }), 0, 3, "file", "test.mp3"),
                DurationMin = 30
            };


            await service.CreateActivity(dto, user);

            // Assert
            var created = await _context.Activite.FirstOrDefaultAsync(a => a.Title == "Test Activity");
            Assert.NotNull(created);
        }


        [Fact]
        public async Task UpdateActivity_UpdatesActivity_WhenActivityExists()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };

            _context.Activite.Add(new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 });
            await _context.SaveChangesAsync();

            var updateActivityDTO = new UpdateActivityDTO
            {
                Title = "UpdatedActivity",
                Description = "UpdatedDescription",
                Status = true
            };

            var result = await service.UpdateActivity(1, updateActivityDTO, user);

            Assert.NotNull(result);
            Assert.Equal("UpdatedActivity", result.Title);
        }

        [Fact]
        public async Task UpdateActivity_ThrowsNotFoundException_WhenActivityDoesNotExist()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };

            var updateActivityDTO = new UpdateActivityDTO
            {
                Title = "UpdatedActivity",
                Description = "UpdatedDescription",
                Status = true
            };

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateActivity(999, updateActivityDTO, user));
        }

        [Fact]
        public async Task DeleteActivity_RemovesActivityFromDatabase()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };

            _context.Activite.Add(new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 });
            await _context.SaveChangesAsync();

            var result = await service.DeleteActivity(1);

            Assert.True(result);
            Assert.Equal(0, await _context.Activite.CountAsync());
        }

        [Fact]
        public async Task ToggleFavorite_TogglesFavoriteStatus()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };
            var activity = new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 };

            _context.Activite.Add(activity);
            await _context.SaveChangesAsync();

            var result = await service.ToggleFavorite(user, 1);

            Assert.True(result);
            var favorite = await _context.SaveActivity.FirstOrDefaultAsync(sa => sa.UserId == user.Id && sa.ActiviteId == activity.Id);
            Assert.NotNull(favorite);
            Assert.True(favorite.IsFavorite);
        }

        [Fact]
        public async Task ToggleToLater_TogglesToLaterStatus()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };
            var activity = new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 };

            _context.Activite.Add(activity);
            await _context.SaveChangesAsync();

            var result = await service.ToggleToLater(user, 1);

            Assert.True(result);
            var toLater = await _context.SaveActivity.FirstOrDefaultAsync(sa => sa.UserId == user.Id && sa.ActiviteId == activity.Id);
            Assert.NotNull(toLater);
            Assert.True(toLater.IsToLater);
        }

        [Fact]
        public async Task GetTopActivities_ReturnsTopActivities()
        {
            var service = new ActivityService(_context, _mockCategoryService.Object);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var user2 = new User { Id = 2, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var category = new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user };
            var activity1 = new Activite { Id = 1, Title = "Activity1", Description = "Description1", Status = true, Category = category, User = user, TypeActitvity = "Type1", ImagePresentation = "http://example.com/image1.jpg", Url = "http://example.com/url1", Duree = 30 };
            var activity2 = new Activite { Id = 2, Title = "Activity2", Description = "Description2", Status = true, Category = category, User = user, TypeActitvity = "Type2", ImagePresentation = "http://example.com/image2.jpg", Url = "http://example.com/url2", Duree = 45 };

            _context.Activite.AddRange(activity1, activity2);
            _context.SaveActivity.Add(new SaveActivity { UserId = user.Id, ActiviteId = activity1.Id, IsFavorite = true });
            _context.SaveActivity.Add(new SaveActivity { UserId = user2.Id, ActiviteId = activity1.Id, IsFavorite = true });
            _context.SaveActivity.Add(new SaveActivity { UserId = user.Id, ActiviteId = activity2.Id, IsFavorite = true });
            await _context.SaveChangesAsync();

            var result = await service.GetTopActivities();

            Assert.NotEmpty(result);
            Assert.Equal("Activity1", result.First().Title);
        }
    }
}
