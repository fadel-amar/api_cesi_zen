using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace CesiZen_API.DTO
{
    public class CreatePageDto
    {
        [Required(ErrorMessage = "Le titre de la page est obligatoire")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Le contenu de la page est obligatoire")]
        public required string Content { get; set; }

        [Url(ErrorMessage = "Le lien doit être un url valide")]
        public string? link { get; set; }
    }

    public class UpdatePageDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Link { get; set; }
        public int? Menu { get; set; }
        public bool? Visibility { get; set; }

    }


    public class ResponsePageDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? link { get; set; }
        public string? type_link { get; set; }
        public bool Visibility { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ResponseListPageDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Visibility { get; set; }
    }

    public class ListPageDto
    {
        public ResponseListPageDto[] Pages { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }


    }



}