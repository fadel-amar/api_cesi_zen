using CesiZen_API.DTO;
using CesiZen_API.Helper;
using CesiZen_API.Mapper;
using CesiZen_API.Models;
using CesiZen_API.Services;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{

    [Route("api/menus")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IUserService _userService;

        public MenuController(IMenuService menuService, IUserService userService)
        {
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService), "MenuSerice n'est pas défini");
            _userService = userService ?? throw new ArgumentNullException(nameof(userService), "UserService n'est pas défini");
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            IEnumerable<Menu> menus = await _menuService.GetAllMenu();
            return Ok(MenuMapper.ToResponseListDto(menus));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            Menu? menu = await _menuService.GetMenuById(id);
            if (menu == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Ce menu n'a pas été trouvé"
                });
            }

            return Ok(MenuMapper.ToResponseFullDto(menu));
        }

        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpPost]

        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto menuDto)
        {
            int? userId = User.GetUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var user = await _userService.GetUserById(userId.Value);

            if (user == null)
            {
                return NotFound(new { message = "Utilisateur introuvable" });
            }

            Menu menuCreated =  await _menuService.CreateMenu(menuDto, user);

            return StatusCode(201, new
            {
                status = 201,
                message = "Le menu a bien été créé",
                data = MenuMapper.ToResponseFullDto(menuCreated)
            });

        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuDto menuDto)
        {

            bool updated = await _menuService.UpdateMenu(id, menuDto);
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
                data = MenuMapper.ToResponseFullDto(await _menuService.GetMenuById(id))
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _menuService.DeleteMenu(id);
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
