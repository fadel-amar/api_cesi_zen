using System;
using System.Collections.Generic;
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
    public class PageServiceTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;

        public PageServiceTests()
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
        public async Task GetAllPages_ReturnsAllPages_WhenIsAdmin()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Page.Add(new Page { Id = 1, Title = "Page1", Content = "Content1", Visibility = true, User = user });
            _context.Page.Add(new Page { Id = 2, Title = "Page2", Content = "Content2", Visibility = false, User = user });
            await _context.SaveChangesAsync();

            var result = await service.GetAllPages(true);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllPages_ReturnsOnlyVisiblePages_WhenNotAdmin()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Page.Add(new Page { Id = 1, Title = "Page1", Content = "Content1", Visibility = true, User = user });
            _context.Page.Add(new Page { Id = 2, Title = "Page2", Content = "Content2", Visibility = false, User = user });
            await _context.SaveChangesAsync();

            var result = await service.GetAllPages(false);

            Assert.Single(result);
            Assert.Equal("Page1", result.First().Title);
        }

        [Fact]
        public async Task GetPagesByIds_ReturnsPages_WhenPagesExist()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Page.Add(new Page { Id = 1, Title = "Page1", Content = "Content1", Visibility = true, User = user });
            _context.Page.Add(new Page { Id = 2, Title = "Page2", Content = "Content2", Visibility = true, User = user });
            await _context.SaveChangesAsync();
            var pagesId = new List<int> { 1, 2 };

            var result = await service.GetPagesByIds(pagesId);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetPagesByIds_ThrowsBadRequestException_WhenPagesIdIsEmpty()
        {
            var service = new PageService(_context);
            var pagesId = new List<int>();

            await Assert.ThrowsAsync<BadRequestException>(() => service.GetPagesByIds(pagesId));
        }

        [Fact]
        public async Task GetPagesByIds_ThrowsNotFoundException_WhenSomePagesDoNotExist()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Page.Add(new Page { Id = 1, Title = "Page1", Content = "Content1", Visibility = true, User = user });
            await _context.SaveChangesAsync();
            var pagesId = new List<int> { 1, 2 };

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetPagesByIds(pagesId));
        }

        [Fact]
        public async Task GetPageById_ReturnsPage_WhenPageExistsAndIsVisible()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Page.Add(new Page { Id = 1, Title = "Page1", Content = "Content1", Visibility = true, User = user });
            await _context.SaveChangesAsync();

            var result = await service.GetPageById(1, false);

            Assert.NotNull(result);
            Assert.Equal("Page1", result.Title);
        }

        [Fact]
        public async Task GetPageById_ThrowsNotFoundException_WhenPageDoesNotExist()
        {
            var service = new PageService(_context);

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetPageById(999, false));
        }

        [Fact]
        public async Task CreatePage_AddsPageToDatabase()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var newPageDto = new CreatePageDto { Title = "NewPage", Content = "NewContent" };

            var result = await service.CreatePage(newPageDto, user);

            Assert.NotNull(result);
            Assert.Equal("NewPage", result.Title);
            Assert.Equal(1, await _context.Page.CountAsync());
        }

        [Fact]
        public async Task UpdatePage_UpdatesPage_WhenPageExists()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Page.Add(new Page { Id = 1, Title = "Page1", Content = "Content1", Visibility = true, User = user });
            await _context.SaveChangesAsync();
            var updatePageDto = new UpdatePageDto { Title = "UpdatedPage", Content = "UpdatedContent", Visibility = true };

            var result = await service.UpdatePage(1, updatePageDto, user);

            Assert.True(result);
            var updatedPage = await _context.Page.FindAsync(1);
            Assert.Equal("UpdatedPage", updatedPage.Title);
        }

        [Fact]
        public async Task UpdatePage_ThrowsNotFoundException_WhenPageDoesNotExist()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            var updatePageDto = new UpdatePageDto { Title = "UpdatedPage", Content = "UpdatedContent", Visibility = true };

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdatePage(999, updatePageDto, user));
        }

        [Fact]
        public async Task DeletePage_RemovesPageFromDatabase()
        {
            var service = new PageService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.Page.Add(new Page { Id = 1, Title = "Page1", Content = "Content1", Visibility = true, User = user });
            await _context.SaveChangesAsync();

            var result = await service.DeletePage(1);

            Assert.True(result);
            Assert.Equal(0, await _context.Page.CountAsync());
        }
    }
}
