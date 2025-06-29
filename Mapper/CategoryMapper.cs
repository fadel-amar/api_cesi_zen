using CesiZen_API.DTO.Response;
using CesiZen_API.Models;

namespace CesiZen_API.Mapper
{
    public static class CategoryMapper
    {
        public static FullCategoryResponseDTO ToResponseFullDto(Category category)
        {
            return new FullCategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                Emoji = category.Emoji,
                Duration = category.Duration,
                Status = category.Visibility,
                Login = category.User?.Login
            };
        }

        public static CategoryResponseDTO ToResponseDto(Category category)
        {
            return new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                Emoji = category.Emoji,
                Duration = category.Duration,
                Status = category.Visibility
            };
        }

        public static List<CategoryResponseDTO> ToResponseListDto(IEnumerable<Category> categories)
        {
            return categories.Select(ToResponseDto).ToList();
        }
    }
}
