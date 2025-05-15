using CesiZen_API.DTO;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{

    [Route("api/menus")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;


        MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            IEnumerable<Menu> menus = await _menuService.GetAllAsync();
            if (!menus.Any())
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Aucun menu n'a été trouvé"
                });
            }

            return Ok(menus);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            Menu? menu = await _menuService.GetByIdAsync(id);
            if (menu == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Ce menu n'a pas été trouvé"
                });
            }

            return Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto menuDto)
        {
            Menu? parentMenu = null;

            if (menuDto.ParentId != null)
            {
                parentMenu = await _menuService.GetByIdAsync(menuDto.ParentId.Value);
                if (parentMenu == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "Le menu parent spécifié n'existe pas"
                    });
                }
            }

            Menu menu = new Menu
            {
                Title = menuDto.Title
            };

            if (parentMenu != null)
            {
                menu.Parent = parentMenu;
            }

            var created = await _menuService.CreateAsync(menu);

            return StatusCode(201, new
            {
                status = 201,
                message = "Le menu a bien été créé",
                data = created
            });
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuDto menuDto)
        {
            Menu? menuExisting = await _menuService.GetByIdAsync(id);
            if (menuExisting == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Ce menu n'existe pas"
                });
            }

            if (menuDto.Title != null) menuExisting.Title = menuDto.Title;
            if (menuDto.Status.HasValue) menuExisting.Status = menuDto.Status.Value;

            if (menuDto.ParentId.HasValue)
            {
                Menu? parentMenu = await _menuService.GetByIdAsync(menuDto.ParentId.Value);
                if (parentMenu == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "Le menu parent spécifié n'existe pas"
                    });
                }

                menuExisting.Parent = parentMenu;
            }

            bool updated = await _menuService.UpdateAsync(menuExisting);

            if (!updated)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Erreur lors de la mise à jour"
                });
            }

            return Ok(new
            {
                status = 200,
                message = "Le menu a bien été mis à jour",
                data = menuExisting
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _menuService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Ce menu n'existe pas ou a déjà été supprimé"
                });
            }

            return Ok(new
            {
                status = 200,
                message = "Le menu a bien été supprimé"
            });
        }
    }
}
