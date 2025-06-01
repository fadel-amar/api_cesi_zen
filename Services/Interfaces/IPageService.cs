using CesiZen_API.DTO;
using CesiZen_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Services.Interfaces
{
    public interface IPageService
    {
        Task<(int totalNumberPages, List<Page> pages)> GetAllPages(int pageNumber, int pageSize, string filter);
        Task<List<Page>> GetPagesByIds( List<int> pagesID);
        Task<Page> GetPageById(int id);
        Task<Page> CreatePage(Page newPage);
        Task<bool> UpdatePage(Page newPage);
        Task<bool> DeletePage(int id);
    }
}
