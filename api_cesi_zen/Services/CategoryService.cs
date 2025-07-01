using CesiZen_API.DTO;
using CesiZen_API.Helper.Exceptions;
using CesiZen_API.ModelBlinders;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CesiZen_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategories(bool isAdmin)
        {
            if (isAdmin)
            {
                return await _context.Category.ToListAsync();
            }
            return await _context.Category.Where(c => c.Visibility).ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id, bool isAdmin)
        {
            Category? category = await _context.Category
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.Visibility));
            if (category == null)
                throw new NotFoundException("La catégorie n'a pas été trouvée");
            return category;
        }

        public async Task<Category> CreateCategory(User user, CreateCategoryDto categoryDto)
        {
            Category category = new()
            {
                Name = categoryDto.Name,
                CreatedAt = DateTime.UtcNow,
                Emoji = categoryDto.Emoji,
                Duration = categoryDto.Duration,
                Visibility = false,
                User = user
            };

            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategory(int id, User user, UpdateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto == null)
            {
                throw new BadRequestException("Les données de la catégorie sont invalides");
            }

            var existing = await GetCategoryById(id, true);
            if (existing == null)
            {
                throw new NotFoundException("La catégorie n'a pas été trouvée");
            }

            existing.Name = updateCategoryDto.Name ?? existing.Name;
            existing.Emoji = updateCategoryDto.Emoji ?? existing.Emoji;
            existing.Duration = updateCategoryDto.Duration ?? existing.Duration;
            existing.Visibility = updateCategoryDto.Status.HasValue ? updateCategoryDto.Status.Value : existing.Visibility;

            await _context.SaveChangesAsync();
            return true;
        }
        

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await GetCategoryById(id, true);
            if (category == null) return false;

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
