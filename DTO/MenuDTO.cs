using System.ComponentModel.DataAnnotations;
using CesiZen_API.Helper.Attributes;
using CesiZen_API.Models;

namespace CesiZen_API.DTO
{
    public class CreateMenuDto
    {
        [Required(ErrorMessage = "Le titre du menu est obligatoire")]
        [UniqueMenuTitle(ErrorMessage = "Ce titre de menu est déjà utilisé.")]
        [MaxLength(50, ErrorMessage = "Le titre du menu ne peut pas dépasser 50 caractères.")]
        public required string Title { get; set; }

        public List<int>? PagesId { get; set; }

        public int? ParentId { get; set; }
    }

    public class UpdateMenuDto
    {
        [UniqueMenuTitle(ErrorMessage = "Ce titre de menu est déjà utilisé.")]
        [MaxLength(50, ErrorMessage = "Le titre du menu ne peut pas dépasser 50 caractères.")]
        public string? Title { get; set; }
        public List<int>? PagesId { get; set; }

        public bool? Status { get; set; }

        public int? ParentId { get; set; }
    }
}
