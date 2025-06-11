using CesiZen_API.DTO;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserById(int id)
        {

            return await _context.User.FindAsync(id);


        }
        public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsers(int pageNumber = 1, int pageSize = 10, string? filter = null)
        {
            var query = _context.User.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.Login.Contains(filter) || u.Email.Contains(filter));
            }

            int totalUsers = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalUsers);

        }
        public async Task<User> CreateUser(User user)
        {

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUser(int id, PatchDTO userDto)
        {
            User? existing = await this.GetUserById(id);
            if (existing == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(userDto.Login))
                existing.Login = userDto.Login;
            if (!string.IsNullOrEmpty(userDto.Email))
                existing.Email = userDto.Email;
            if (!string.IsNullOrEmpty(userDto.Role))
                existing.Role = userDto.Role;
            if (userDto.Disabled.HasValue)
                existing.Disabled = userDto.Disabled.Value;
            if (userDto.Banned.HasValue)
                existing.Banned = userDto.Banned.Value;

            existing.UpdatedAt = DateTime.UtcNow;

            _context.User.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<User> UpdateMyAccount(User user, UpdateMyAccontDTO updateMyAccontDTO)
        {
            user.Login = updateMyAccontDTO.Login ?? user.Login;
            user.Email = updateMyAccontDTO.Email ?? user.Email;
            user.Disabled = updateMyAccontDTO.Disabled ?? user.Disabled;
            user.UpdatedAt = DateTime.UtcNow;
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUser(int id)
        {
            User? existing = await this.GetUserById(id);
            if (existing == null)
            {
                throw new KeyNotFoundException("Utilisateur non trouvé");
            }

            _context.User.Remove(existing);
            return true;
        }

        public async Task<User> GetUserByIdentifier(string identifier)
        {
            try
            {
                User? user = await _context.User.FirstOrDefaultAsync(u => u.Email == identifier || u.Login == identifier);
                if (user == null)
                {
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur s'est produite" + ex.Message);
            }

        }
    }
}
