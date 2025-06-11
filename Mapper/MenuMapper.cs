using CesiZen_API.DTO.Response.MenuResponse;
using CesiZen_API.DTO.Response.userResponse;
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
                Parent = ToParentDto(menu.Parent),
                SousMenus = menu.SousMenus?.Select(ToSousMenuDto).ToList() ?? new(),
                Pages = menu.Pages.Select(ToPageDto).ToList(),
                User = toUserDto(menu.User),
                CreatedAt = menu.CreatedAt,
            };
        }

        public static ParentMenuResponseDTO ToParentDto(Menu menu)
        {
            if (menu == null) return null;
            return new ParentMenuResponseDTO
            {
                Id = menu.Id,
                Title = menu.Title
            };
        }
        public static SousMenuResponseDTO ToSousMenuDto(Menu menu)
        {
            if (menu == null) return null;
            return new SousMenuResponseDTO
            {
                Id = menu.Id,
                Title = menu.Title
            };
        }
        public static PagesMenuResponseDTO ToPageDto(Page page)
        {
            if (page == null) return null;
            return new PagesMenuResponseDTO
            {
                Id = page.Id,
                Title = page.Title
            };
        }

        public static UserShortReponseDTO toUserDto(User user)
        {
            if (user == null) return null;
            return new UserShortReponseDTO
            {
                Id = user.Id,
                username = user.Login
            };
        }

        public static MenuResponseDTO ToResponseDto(Menu menu)
        {
            return new MenuResponseDTO
            {
                Id = menu.Id,
                Title = menu.Title,
                Parent = ToParentDto(menu.Parent),
                SousMenus = menu.SousMenus?.Select(ToSousMenuDto).ToList() ?? new(),
                Pages = menu.Pages.Select(ToPageDto).ToList(),
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
