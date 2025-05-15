using CesiZen_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Services.Interfaces
{
    public interface IPageService
    {
        Task<(int totalNumberPages, List<Page> pages)> GetAllAsync(int pageNumber, int pageSize, string filter);
        Task<Page> GetByIdAsync(int id);
        Task<Page> CreateAsync(Page page);
        Task<bool> UpdateAsync(Page page);
        Task<bool> DeleteAsync(int id);
    }
}
