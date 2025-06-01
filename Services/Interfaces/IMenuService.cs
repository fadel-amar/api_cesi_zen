using CesiZen_API.DTO;
using CesiZen_API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CesiZen_API.Services.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetAllMenu();
        Task<Menu> GetMenuById(int id);
        Task<Menu> GetMenuByTitle(string title);
        Task<Menu> CreateMenu(CreateMenuDto menu, User user);
        Task<bool> UpdateMenu(int id, UpdateMenuDto menu);
        Task<bool> DeleteMenu(int id);     
    }
}
