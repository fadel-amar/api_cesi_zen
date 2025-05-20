using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.DTO
{
    //TODO Inclure les pages
    public class CreateMenuDto
    {
        [Required(ErrorMessage = "Le titre du menu est obligatoire")]
        public string Title { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }

    public class UpdateMenuDto
    {
        public string? Title { get; set; }

        public int? Status { get; set; }

        public int? ParentId { get; set; }
    }
}
