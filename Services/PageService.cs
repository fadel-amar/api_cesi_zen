using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using CesiZen_API.Helper.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_API.Services
{
    public class PageService : IPageService
    {
        private readonly AppDbContext _context;

        public PageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(int totalNumberPages, List<Page> pages)> GetAllPages(int pageNumber, int pageSize, string filter)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new BadRequestException("Le numéro de page et la taille doivent être supérieurs à zéro.");

            IQueryable<Page> query = _context.Page.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
                query = query.Where(p => p.Title.Contains(filter));

            int totalPages = await query.CountAsync();

            List<Page> pages = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalPages, pages);
        }

        public async Task<List<Page>> GetPagesByIds(List<int> pagesId)
        {
            if (pagesId == null || !pagesId.Any())
                throw new BadRequestException("La liste des IDs de pages ne peut pas être vide.");

            var pages = await _context.Page
                .Where(p => pagesId.Contains(p.Id))
                .ToListAsync();

            if (pages.Count != pagesId.Count)
                throw new NotFoundException("Certaines pages n'existent pas.");

            return pages;
        }

        public async Task<Page> GetPageById(int id)
        {
            var page = await _context.Page.FindAsync(id);
            if (page == null)
                throw new NotFoundException($"La page avec l'ID {id} n'existe pas.");

            return page;
        }

        public async Task<Page> CreatePage(Page newPage)
        {
            if (string.IsNullOrWhiteSpace(newPage.Title))
                throw new BadRequestException("Le titre de la page est obligatoire.");

            _context.Page.Add(newPage);
            await _context.SaveChangesAsync();
            return newPage;
        }

        public async Task<bool> UpdatePage(Page newPage)
        {
            var existing = await _context.Page.FindAsync(newPage.Id);
            if (existing == null)
                throw new NotFoundException("La page à modifier n'existe pas.");

            existing.Title = newPage.Title;
            existing.Menu = newPage.Menu;
            existing.Content = newPage.Content;
            existing.Visibility = newPage.Visibility;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePage(int id)
        {
            var existing = await _context.Page.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("La page à supprimer n'existe pas.");

            _context.Page.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
