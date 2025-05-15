using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.DTO
{
    public class CategoryDTO
    {

        public class CreateCategoryDto
        {
            [Required(ErrorMessage = "Le nom de la catégorie est obligatoire")]
            [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Le nom de la catégorie doit contenir que des lettres.")]
            public string Name { get; set; }
        }
        public class UpdateCategoryDto
        {
            [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Le nom de la catégorie doit contenir que des lettres.")]
            public string Name { get; set; }
            public int Status { get; set; }
        }

    }
}
