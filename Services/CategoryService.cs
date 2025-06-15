using CesiZen_API.Helper.Exceptions;
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

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            Category? caategory =  await _context.Category.FindAsync(id);
            if (caategory == null)
                throw new NotFoundException("La catégorie n'as pas été trouvé");
            return caategory;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            var existing = await _context.Category.FindAsync(category.Id);
            if (existing == null)
                return false;

            existing.Name = category.Name; 

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null) return false;

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
