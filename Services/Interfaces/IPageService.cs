using CesiZen_API.DTO;
using CesiZen_API.Models;

namespace CesiZen_API.Services.Interfaces
{
    public interface IPageService
    {
        Task<IEnumerable<Page>> GetAllPages(bool isAdmin);
        Task<List<Page>> GetPagesByIds( List<int> pagesID);
        Task<Page> GetPageById(int id, bool isAdmin);
        Task<Page> CreatePage(CreatePageDto newPageDto, User user);
        Task<bool> UpdatePage(int id, UpdatePageDto updatedPage, User user);
        Task<bool> DeletePage(int id);
    }
}
