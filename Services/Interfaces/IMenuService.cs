using CesiZen_API.DTO;
using CesiZen_API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CesiZen_API.Services.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<Menu> GetByIdAsync(int id);
        Task<Menu> CreateAsync(CreateMenuDto menu);
        Task<bool> UpdateAsync(int id, UpdateMenuDto menu);
        Task<bool> DeleteAsync(int id);     
    }
}
