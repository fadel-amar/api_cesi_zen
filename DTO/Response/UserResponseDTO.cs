namespace CesiZen_API.DTO.Response.userResponse
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public bool Banned { get; set; }
        public bool Disabled { get; set; }
        public string Role { get; set; }
    }

    public class FullUserResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public bool Banned { get; set; }
        public bool Disabled { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ListUserResponseDTO
    {
        public List<UserResponseDTO> Users { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalNumberUser { get; set; }
        public int TotalPages { get; set; }
    }
}
