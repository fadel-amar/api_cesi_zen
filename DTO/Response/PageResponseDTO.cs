using CesiZen_API.DTO.Response.userResponse;

namespace CesiZen_API.DTO.Response
{
    public class PageResponseDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public required Boolean Visibility { get; set; }

    }

    public class FullPageResponsDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public Boolean Visibility { get; set; }
        public string VideoLink { get; set; }
        public required UserShortReponseDTO User { get; set; }
        public DateTime CreatedAt { get; set; }

    }


    public class ListPageResponseDTO
    {
        public IEnumerable<PageResponseDTO> Pages { get; set; }
    }
        
}
