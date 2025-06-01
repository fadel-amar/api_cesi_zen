using System.Data;
using CesiZen_API.DTO;
using CesiZen_API.Helper.Exceptions;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_API.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;
        private readonly IPageService _pageService;

        public MenuService(AppDbContext context, IPageService pageService)
        {
            _context = context;
            _pageService = pageService;
        }

        public async Task<IEnumerable<Menu>> GetAllMenu()
        {
            return await _context.Menu
                .Include(m => m.Parent)
                .Include(m => m.SousMenus)
                .Include(m => m.Pages)
                .Include(m => m.User)
                .ToListAsync();
        }

        public async Task<Menu> GetMenuById(int id)
        {
            var menu = await _context.Menu
               .Include(m => m.Parent)
               .Include(m => m.SousMenus)
               .Include(m => m.Pages)
               .Include(m => m.User)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (menu == null)
                throw new NotFoundException("Menu introuvable.");
            return menu;
        }

        public async Task<Menu?> GetMenuByTitle(string title)
        {
            return await _context.Menu.FirstOrDefaultAsync(m => m.Title == title);
        }

        public async Task<Menu> CreateMenu(CreateMenuDto newMenuDto, User user)
        {
            Menu? parentMenu = null;

            if (newMenuDto.ParentId != null)
            {
                parentMenu = await _context.Menu.FindAsync(newMenuDto.ParentId.Value);
                if (parentMenu == null)
                {
                    throw new NotFoundException("Le menu parent spécifié n'existe pas.");
                }
            }

            List<Page> pages = new();
            if (newMenuDto.PagesId != null && newMenuDto.PagesId.Any())
            {
                pages = await _pageService.GetPagesByIds(newMenuDto.PagesId);
            }

            Menu menu = new Menu
            {
                Title = newMenuDto.Title,
                User = user,
                Status = 1,
                Parent = parentMenu,
                Pages = pages
            };

            _context.Menu.Add(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task<bool> UpdateMenu(User user, int id, UpdateMenuDto menuDto)
        {
            if (menuDto is null)
                throw new ArgumentNullException(nameof(menuDto));

            var isDtoEmpty =
                menuDto.Title == null &&
                !menuDto.Status.HasValue &&
                !menuDto.ParentId.HasValue &&
                (menuDto.PagesId == null || !menuDto.PagesId.Any());

            if (isDtoEmpty)
                throw new BadRequestException("Aucune donnée à mettre à jour.");

            var menu = await _context.Menu
                .Include(m => m.Pages)
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menu == null)
                throw new NotFoundException("Ce menu n'existe pas.");

            if (menuDto.Title != null)
                menu.Title = menuDto.Title;

            if (menuDto.Status.HasValue)
                menu.Status = menuDto.Status.Value;

            if (menuDto.ParentId.HasValue && menuDto.ParentId.Value != menu.Parent?.Id)
            {
                var parent = await _context.Menu.FindAsync(menuDto.ParentId.Value);
                if (parent == null)
                    throw new NotFoundException("Le menu parent spécifié n'existe pas.");
                menu.Parent = parent;
            }

            if (menuDto.PagesId?.Any() == true)
            {
                var pages = await _pageService.GetPagesByIds(menuDto.PagesId);
                menu.Pages = pages;
            }

            menu.User = user;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteMenu(int id)
        {
            var menu = await _context.Menu.FindAsync(id);
            if (menu == null)
                throw new NotFoundException("Le menu n'a pas été trouvé.");

            _context.Menu.Remove(menu);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
