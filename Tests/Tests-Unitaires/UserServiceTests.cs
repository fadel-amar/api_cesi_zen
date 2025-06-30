using System;
using System.Threading.Tasks;
using CesiZen_API.DTO;
using CesiZen_API.Helper.Exceptions;
using CesiZen_API.Models;
using CesiZen_API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;

        public UserServiceTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
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
        public async Task GetUserById_ReturnsUser_WhenUserExists()
        {
            var service = new UserService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var result = await service.GetUserById(1);

            Assert.NotNull(result);
            Assert.Equal("testuser", result.Login);
        }

        [Fact]
        public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
        {
            var service = new UserService(_context);
            var result = await service.GetUserById(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUser_AddsUserToDatabase()
        {
            var service = new UserService(_context);
            var newUser = new User { Login = "newuser", Email = "new@example.com", Password = "password", Role = "user" };

            var result = await service.CreateUser(newUser);

            Assert.NotNull(result);
            Assert.Equal("newuser", result.Login);
            Assert.Equal(1, await _context.User.CountAsync());
        }

        [Fact]
        public async Task UpdateUser_UpdatesUser_WhenUserExists()
        {
            var service = new UserService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new PatchDTO { Login = "updateduser", Email = "updated@example.com" };

            var result = await service.UpdateUser(1, userDto);

            Assert.NotNull(result);
            Assert.Equal("updateduser", result.Login);
            Assert.Equal("updated@example.com", result.Email);
        }

        [Fact]
        public async Task UpdateUser_ThrowsNotFoundException_WhenUserDoesNotExist()
        {
            var service = new UserService(_context);
            var userDto = new PatchDTO { Login = "updateduser", Email = "updated@example.com" };

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateUser(999, userDto));
        }

        [Fact]
        public async Task DeleteUser_RemovesUserFromDatabase()
        {
            var service = new UserService(_context);
            var user = new User { Id = 1, Login = "testuser", Email = "test@example.com", Password = "password", Role = "user" };
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var result = await service.DeleteUser(1);

            Assert.True(result);
            Assert.Equal(0, await _context.User.CountAsync());
        }
    }
}
