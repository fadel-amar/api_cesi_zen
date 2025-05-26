using CesiZen_API.Models;

namespace CesiZen_API.DTO.Response.MenuResponse
{
    public class MenuResponseDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public Menu? Parent { get; set; }
        public required int Status { get; set; }
    }

    public class FullMenuResponseDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public int Status { get; set; }
        public DateTime DateCreation { get; set; }
        public Menu? Parent { get; set; }
        public ICollection<Menu> SousMenus { get; set; } = new List<Menu>();
        public ICollection<Page> Pages { get; set; } = new List<Page>();
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ListMenuResponseDTO
    {
        public List<MenuResponseDTO> Menus { get; set; }
    }
}
