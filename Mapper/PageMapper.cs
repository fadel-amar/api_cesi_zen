using CesiZen_API.DTO.Response;
using CesiZen_API.DTO.Response.MenuResponse;
using CesiZen_API.DTO.Response.userResponse;
using CesiZen_API.Models;

namespace CesiZen_API.Mapper
{
    public static class PageMapper
    {

        public static FullPageResponsDTO ToResponseFullDto(Page page)
        {
            return new FullPageResponsDTO
            {
                Id = page.Id,
                Title = page.Title,
                Content = page.Content,
                VideoLink = page.link,
                Visibility = page.Visibility,
                User = toUserDto(page.User),
                CreatedAt = page.CreatedAt,

            };
        }

        public static PageResponseDTO ToResponseDto(Page page)
        {
            return new PageResponseDTO
            {
                Id = page.Id,
                Title = page.Title,
                Visibility = page.Visibility,

            };
        }

        public static ListPageResponseDTO ToResponseListDto(IEnumerable<Page> pages)
        {
            return new ListPageResponseDTO
            {
                Pages = pages.Select(ToResponseDto).ToList()
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
    }
}
