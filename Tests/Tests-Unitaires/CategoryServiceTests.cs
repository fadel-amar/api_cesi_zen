using System;
using System.Linq;
using System.Threading.Tasks;
using CesiZen_API.DTO;
using CesiZen_API.Helper.Exceptions;
using CesiZen_API.Models;
using CesiZen_API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Services
{
    public class CategoryServiceTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;

        public CategoryServiceTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(_options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllCategories_ReturnsAllCategories_WhenIsAdmin()
        {
            var service = new CategoryService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Category.Add(new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user });
            _context.Category.Add(new Category { Id = 2, Name = "Category2", Emoji = "😊", Duration = "60", Visibility = false, User = user });
            await _context.SaveChangesAsync();

            var result = await service.GetAllCategories(true);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOnlyVisibleCategories_WhenNotAdmin()
        {
            var service = new CategoryService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Category.Add(new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user });
            _context.Category.Add(new Category { Id = 2, Name = "Category2", Emoji = "😊", Duration = "60", Visibility = false, User = user });
            await _context.SaveChangesAsync();

            var result = await service.GetAllCategories(false);

            Assert.Single(result);
            Assert.Equal("Category1", result.First().Name);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsCategory_WhenCategoryExistsAndIsVisible()
        {
            var service = new CategoryService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Category.Add(new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user });
            await _context.SaveChangesAsync();

            var result = await service.GetCategoryById(1, false);

            Assert.NotNull(result);
            Assert.Equal("Category1", result.Name);
        }

        [Fact]
        public async Task GetCategoryById_ThrowsNotFoundException_WhenCategoryDoesNotExist()
        {
            var service = new CategoryService(_context);

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetCategoryById(999, false));
        }

        [Fact]
        public async Task CreateCategory_AddsCategoryToDatabase()
        {
            var service = new CategoryService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var categoryDto = new CreateCategoryDto { Name = "NewCategory", Emoji = "😊", Duration = "60" };

            var result = await service.CreateCategory(user, categoryDto);

            Assert.NotNull(result);
            Assert.Equal("NewCategory", result.Name);
            Assert.Equal(1, await _context.Category.CountAsync());
        }

        [Fact]
        public async Task UpdateCategory_UpdatesCategory_WhenCategoryExists()
        {
            var service = new CategoryService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Category.Add(new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user });
            await _context.SaveChangesAsync();
            var updateCategoryDto = new UpdateCategoryDto { Name = "UpdatedCategory", Emoji = "😊", Duration = "60", Status = true };

            var result = await service.UpdateCategory(1, user, updateCategoryDto);

            Assert.True(result);
            var updatedCategory = await _context.Category.FindAsync(1);
            Assert.Equal("UpdatedCategory", updatedCategory.Name);
        }

        [Fact]
        public async Task UpdateCategory_ThrowsNotFoundException_WhenCategoryDoesNotExist()
        {
            var service = new CategoryService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var updateCategoryDto = new UpdateCategoryDto { Name = "UpdatedCategory", Emoji = "😊", Duration = "60", Status = true };

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateCategory(999, user, updateCategoryDto));
        }

        [Fact]
        public async Task DeleteCategory_RemovesCategoryFromDatabase()
        {
            var service = new CategoryService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Category.Add(new Category { Id = 1, Name = "Category1", Emoji = "😊", Duration = "60", Visibility = true, User = user });
            await _context.SaveChangesAsync();

            var result = await service.DeleteCategory(1);

            Assert.True(result);
            Assert.Equal(0, await _context.Category.CountAsync());
        }
    }
}
