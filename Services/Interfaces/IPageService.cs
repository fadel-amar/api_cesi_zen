using CesiZen_API.DTO;
using CesiZen_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Services.Interfaces
{
    public interface IPageService
    {
        Task<(int totalNumberPages, List<Page> pages)> GetAllAsync(int pageNumber, int pageSize, string filter);
        Task<Page> GetByIdAsync(int id);
        Task<Page> CreateAsync(Page newPage);
        Task<bool> UpdateAsync(Page newPage);
        Task<bool> DeleteAsync(int id);
    }
}
