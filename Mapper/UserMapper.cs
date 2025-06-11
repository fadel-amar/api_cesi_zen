using CesiZen_API.DTO.Response.userResponse;
using CesiZen_API.Models;


namespace CesiZen_API.Mappers
{
    public static class UserMapper
    {
        public static FullUserResponseDTO toResponseFullDto(User user)
        {
            return new FullUserResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                Login = user.Login,
                Banned = user.Banned,
                Disabled = user.Disabled,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
        public static UserResponseDTO ToResponseDto(User user)
        {
       
             return new UserResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                Login = user.Login,
                Banned = user.Banned,
                Disabled = user.Disabled,
                Role = user.Role
            };
        }
        public static ListUserResponseDTO ToResponseListDto(
            IEnumerable<User> users,
            int pageNumber,
            int pageSize,
            int totalNumberUser)
        {
            return new ListUserResponseDTO
            {
                Users = users.Select(ToResponseDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalNumberUser = totalNumberUser,
                TotalPages = (int)Math.Ceiling((double)totalNumberUser / pageSize)
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
