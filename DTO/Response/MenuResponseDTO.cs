using CesiZen_API.DTO.Response.userResponse;
using CesiZen_API.Models;

namespace CesiZen_API.DTO.Response.MenuResponse
{
    public class MenuResponseDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public ParentMenuResponseDTO? Parent { get; set; }
        public List<PagesMenuResponseDTO>? Pages { get; set; }
        public List<SousMenuResponseDTO>? SousMenus { get; set; }
        public required bool Status { get; set; }
    }

    public class FullMenuResponseDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public bool Status { get; set; }
        public DateTime DateCreation { get; set; }
        public ParentMenuResponseDTO? Parent { get; set; }
        public List<PagesMenuResponseDTO>? Pages { get; set; }
        public List<SousMenuResponseDTO>? SousMenus { get; set; }
        public UserShortReponseDTO User { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ListMenuResponseDTO
    {
        public List<MenuResponseDTO> Menus { get; set; }
    }
    public class ParentMenuResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class SousMenuResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }


    public class PagesMenuResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }


}
