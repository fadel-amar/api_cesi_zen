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

        public MenuService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Menu>> GetAllMenu()
        {
            return await _context.Menu.ToListAsync();
        }

        public async Task<Menu> GetMenuById(int id)
        {
            var menu = await _context.Menu.FindAsync(id);
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

            Menu menu = new Menu
            {
                Title = newMenuDto.Title,
                User = user,
                Status = 1,
                Parent = parentMenu
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
