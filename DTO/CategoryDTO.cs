using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.DTO
{
    public class CategoryDTO
    {
        [Required(ErrorMessage = "Le nom de la catégorie est obligatoire")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Le nom de la catégorie doit contenir que des lettres.")]
        public string Name { get; set; }
    }
}
