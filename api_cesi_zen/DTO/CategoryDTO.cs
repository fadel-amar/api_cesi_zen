using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.DTO
{

    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Le nom de la catégorie est obligatoire")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Le nom de la catégorie doit contenir que des lettres.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "L'emoji de la catégorie est obligatoire")]
        public required string Emoji { get; set; }

        [Required(ErrorMessage = "La durée moyen des activités de ce type est requis")]
        [RegularExpression(@"^\d+\sà\s\d+\sminutes$", ErrorMessage = "La durée doit être au format '3 à 5 minutes'.")]
        public required string Duration { get; set; }
    }
           public class UpdateCategoryDto
        {
            [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Le nom de la catégorie doit contenir que des lettres.")]
            public string? Name { get; set; }
            public string? Emoji { get; set; }
            [RegularExpression(@"^\d+\sà\s\d+\sminutes$", ErrorMessage = "La durée doit être au format '3 à 5 minutes'.")]
            public string? Duration { get; set; }
            public Boolean? Status { get; set; }
        }
}
