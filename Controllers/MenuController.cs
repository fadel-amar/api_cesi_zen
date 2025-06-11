using CesiZen_API.DTO;
using CesiZen_API.Helper;
using CesiZen_API.Mapper;
using CesiZen_API.ModelBlinders;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{
    [Route("api/menus")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService, IUserService userService)
        {
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            var menus = await _menuService.GetAllMenu();
            return Ok(MenuMapper.ToResponseListDto(menus));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            var menu = await _menuService.GetMenuById(id);
            return Ok(MenuMapper.ToResponseFullDto(menu));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMenu([CurrentUser] User user, [FromBody] CreateMenuDto menuDto)
        {
            if (user == null)
                return Unauthorized(new { message = "Utilisateur non authentifié" });

            var menu = await _menuService.CreateMenu(menuDto, user);

            return StatusCode(201, new
            {
                status = 201,
                message = "Le menu a bien été créé",
                data = MenuMapper.ToResponseFullDto(menu)
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [CurrentUser] User user, [FromBody] UpdateMenuDto menuDto)
        {

            if (user == null)
                return Unauthorized(new { message = "Utilisateur non authentifié" });


            await _menuService.UpdateMenu(user, id, menuDto);
            var updatedMenu = await _menuService.GetMenuById(id);

            return Ok(new
            {
                status = 200,
                message = "Le menu a bien été mis à jour",
                data = MenuMapper.ToResponseFullDto(updatedMenu)
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _menuService.DeleteMenu(id);

            return Ok(new
            {
                status = 200,
                message = "Le menu a bien été supprimé"
            });
        }
    }
}
