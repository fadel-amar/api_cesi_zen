using CesiZen_API.DTO;
using CesiZen_API.Models;

namespace CesiZen_API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories(bool isAdmin);
        Task<Category> GetCategoryById(int id, bool isAdmin);
        Task<Category> CreateCategory(User user, CreateCategoryDto createCategoryDto);
        Task<bool> UpdateCategory(int id , User user, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategory(int id);
    }
}
