using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using CesiZen_API.Helper.Exceptions;
using Microsoft.EntityFrameworkCore;
using CesiZen_API.DTO;

namespace CesiZen_API.Services
{
    public class PageService : IPageService
    {
        private readonly AppDbContext _context;

        public PageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Page>> GetAllPages()
        {
            var pages = await _context.Page
                .ToListAsync();

            return pages;
        }

        public async Task<List<Page>> GetPagesByIds(List<int> pagesId)
        {
            if (pagesId == null || !pagesId.Any())
                throw new BadRequestException("La liste des IDs de pages ne peut pas être vide.");

            var pages = await _context.Page
                .Include(p => p.User)
                .Where(p => pagesId.Contains(p.Id))
                .ToListAsync();

            if (pages.Count != pagesId.Count)
                throw new NotFoundException("Certaines pages n'existent pas.");

            return pages;
        }

        public async Task<Page> GetPageById(int id)
        {
            var page = await _context.Page
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (page == null)
                throw new NotFoundException("Cette page n'a pas été trouvée.");

            return page;
        }

        public async Task<Page> CreatePage(CreatePageDto newPageDto, User user)
        {
            Page newPage = new Page
            {
                Title = newPageDto.Title,
                Content = newPageDto.Content,
                User = user
            };
            if (newPageDto.link != null)
            {
                newPage.link = newPageDto.link;
            }

            _context.Page.Add(newPage);
            await _context.SaveChangesAsync();
            return newPage;
        }

        public async Task<bool> UpdatePage(int id, UpdatePageDto updatedPage, User user)
        {
            if (updatedPage == null)
                throw new BadRequestException("Le DTO de mise à jour ne peut pas être null.");

            Page? existing = await _context.Page.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("La page à modifier n'existe pas.");

            existing.Title = updatedPage.Title ?? existing.Title;
            existing.Content = updatedPage.Content ?? existing.Content;
            existing.Visibility = updatedPage.Visibility ?? existing.Visibility;
            existing.link = updatedPage.Link ?? existing.link;
            existing.User = user;

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
