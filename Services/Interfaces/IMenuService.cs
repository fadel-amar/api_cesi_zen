using CesiZen_API.Models;

namespace CesiZen_API.Services.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<Menu> GetByIdAsync(int id);
        Task<Menu> CreateAsync(Menu menu);
        Task<bool> UpdateAsync(Menu menu);
        Task<bool> DeleteAsync(int id);     
    }
}
