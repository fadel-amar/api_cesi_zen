using CesiZen_API.DTO.Response.MenuResponse;
using CesiZen_API.Models;

namespace CesiZen_API.Mapper
{
    public static class MenuMapper
    {
        public static FullMenuResponseDTO ToResponseFullDto(Menu menu)
        {
            return new FullMenuResponseDTO
            {
                Id = menu.Id,
                Title = menu.Title,
                Status = menu.Status,
                DateCreation = menu.CreatedAt,
                Parent = menu.Parent,
                SousMenus = menu.SousMenus,
                Pages = menu.Pages,
                User = menu.User,
                CreatedAt = menu.CreatedAt,
            };
        }

        public static MenuResponseDTO ToResponseDto(Menu menu)
        {
            return new MenuResponseDTO
            {
                Id = menu.Id,
                Title = menu.Title,
                Parent = menu.Parent,
                Status = menu.Status
            };
        }

        public static ListMenuResponseDTO ToResponseListDto(IEnumerable<Menu> menus)
        {
            return new ListMenuResponseDTO
            {
                Menus = menus.Select(ToResponseDto).ToList()
            };
        }
    }
}
