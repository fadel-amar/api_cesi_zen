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

        public async Task<bool> UpdateMenu(int id, UpdateMenuDto menuDto)
        {
            var menuExisting = await _context.Menu.FindAsync(id);
            if (menuExisting == null)
                throw new NotFoundException("Ce menu n'existe pas.");

            if (menuDto.Title != null)
                menuExisting.Title = menuDto.Title;

            if (menuDto.Status.HasValue)
                menuExisting.Status = menuDto.Status.Value;

            if (menuDto.ParentId.HasValue)
            {
                var parentMenu = await _context.Menu.FindAsync(menuDto.ParentId.Value);
                if (parentMenu == null)
                    throw new NotFoundException("Le menu parent spécifié n'existe pas.");

                menuExisting.Parent = parentMenu;
            }

            if (menuDto.PagesId != null)
            {
                var newPages = await _pageService.GetPagesByIds(menuDto.PagesId);
                menuExisting.Pages = newPages;
            }


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
