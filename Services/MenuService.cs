using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_API.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;
        public MenuService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menu.ToListAsync();
        }
        public async Task<Menu> GetByIdAsync(int id)
        {
            return await _context.Menu.FindAsync(id);
        }
        public async Task<Menu> CreateAsync(Menu menu)
        {
            _context.Menu.Add(menu);
            await _context.SaveChangesAsync();
            return menu;
        }
        public async Task<bool> UpdateAsync(Menu menu)
        {
            var existing = await _context.Menu.FindAsync(menu.Id);
            if (existing == null)
                return false;

            existing.Title = menu.Title;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var menu = await _context.Menu.FindAsync(id);
            if (menu == null) return false;
            _context.Menu.Remove(menu);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
