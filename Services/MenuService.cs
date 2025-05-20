using System.Data;
using CesiZen_API.DTO;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menu.ToListAsync();
        }
        public async Task<Menu> GetByIdAsync(int id)
        {
            return await _context.Menu.FindAsync(id);
        }
        public async Task<Menu> CreateAsync(CreateMenuDto newMenuDto)
        {
            try
            {
                Menu? parentMenu = null;

                if (newMenuDto.ParentId != null)
                {
                    parentMenu = await this.GetByIdAsync(newMenuDto.ParentId.Value);
                    if (parentMenu == null)
                    {
                        throw new BadHttpRequestException("Le menu parent spécifié n'existe pas.");
                    }
                }

                Menu menu = new Menu
                {
                    Title = newMenuDto.Title
                };

                if (parentMenu != null)
                {
                    menu.Parent = parentMenu;
                }

                _context.Menu.Add(menu);
                await _context.SaveChangesAsync();
                return menu;

            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException("Erreur de requête : " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur interne est survenue : " + ex.Message);
            }
        }
        public async Task<bool> UpdateAsync(int id, UpdateMenuDto menuDto)
        {
            try
            {
                Menu? menuExisting = await this.GetByIdAsync(id);
                if (menuExisting == null)
                {
                    throw new KeyNotFoundException("Ce menu n'existe pas.");
                }

                if (menuDto.Title != null) menuExisting.Title = menuDto.Title;
                if (menuDto.Status.HasValue) menuExisting.Status = menuDto.Status.Value;

                if (menuDto.ParentId.HasValue)
                {
                    Menu? parentMenu = await this.GetByIdAsync(menuDto.ParentId.Value);
                    if (parentMenu == null)
                    {
                        throw new KeyNotFoundException("Le menu parent spécifié n'existe pas.");
                    }

                    menuExisting.Parent = parentMenu;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception($"Erreur : {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue : " + ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var menu = await _context.Menu.FindAsync(id);
            if (menu == null) return false;
            _context.Menu.Remove(menu);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
