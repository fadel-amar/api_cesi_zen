using System.ComponentModel;
using System.Data;
using CesiZen_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_API.Services
{
    public class PageService
    {
        private readonly AppDbContext _context;
        public PageService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<(int totalNumberPages, List<Page> pages)> GetAllAsync(int pageNumber, int pageSize, string filter)
        {
            IQueryable<Page> query = _context.Page.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.Title.Contains(filter));
            }


             int totalPages= await query.CountAsync();


            List<Page> pages = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalPages, pages);
        }

        public async Task<Page> GetByIdAsync(int id )
        {
            return await _context.Page.FindAsync( id );
        }

        public async Task<Page> CreateAsync(Page newPage)
        {
            _context.Page.Add(newPage);
            await _context.SaveChangesAsync();
            return newPage;
        }

        public async Task<bool> UpdateAsync(Page newPage)
        {
            var existing = await _context.Page.FindAsync(newPage.Id);
            if (existing == null)
                return false;

            existing.Title = newPage.Title;
            existing.Menu = newPage.Menu;
            existing.Content = newPage.Content;
            existing.Visibility = newPage.Visibility;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteById(int id)
        {
            var existing = await _context.Page.FindAsync(id);
            if (existing != null)
            {
                _context.Page.Remove(existing);
            } else
            {
                return false;
            }

            return true;
        }  
    }

}
