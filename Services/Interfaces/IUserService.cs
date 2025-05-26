using CesiZen_API.DTO;
using CesiZen_API.Models;

namespace CesiZen_API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserById(int id);
        Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsers(int pageNumber = 1, int pageSize = 10, string? filter = null);
        Task<User> CreateUser(User user);
        Task<User?> UpdateUser(int id, PatchDTO userDto);
        Task<bool> DeleteUser(int id);
        Task<User?> GetUserByIdentifier(string identifier);
    }
}
