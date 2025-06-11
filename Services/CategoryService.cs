using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Category.FindAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existing = await _context.Category.FindAsync(category.Id);
            if (existing == null)
                return false;

            existing.Name = category.Name; 

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null) return false;

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
