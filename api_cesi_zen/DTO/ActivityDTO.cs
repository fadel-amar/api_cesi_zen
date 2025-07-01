using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CesiZen_API.DTO
{
    public class CreateActivityDTO
    {
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "La description est obligatoire.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "La durée est obligatoire.")]
        [Range(1, 120, ErrorMessage = "La durée doit être supérieure à zéro.")]
        public required int DurationMin { get; set; }

        [Required(ErrorMessage = "L'image de présentation est obligatoire.")]
        public required IFormFile ImagePresentation { get; set; }

        [Required(ErrorMessage = "Le contenu de l'activité est obligatoire.")]
        public required IFormFile Url { get; set; }

        [Required(ErrorMessage = "Le type d'activité est obligatoire.")]
        [Range(1, int.MaxValue, ErrorMessage = "Le type d'activité doit être supérieur à zéro.")]
        public required int TypeActivitty { get; set; }

        [Required(ErrorMessage = "L'ID de la catégorie est obligatoire.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID de la catégorie doit être supérieur à zéro.")]
        public required int CategoryId { get; set; }
    }
    public class UpdateActivityDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Le type d'activité doit être supérieur à zéro.")]
        public int? DurationMin { get; set; }
        public IFormFile? Url { get; set; }
        public IFormFile? ImagePresentation { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "L'ID de la catégorie doit être supérieur à zéro.")]
        public int? CategoryId { get; set; }
        public int? TypeActivitty { get; set; }
        public bool? Status { get; set; }
    }
    public class FilterActivity
    {
        public string? title { get; set; }
        public int? category { get; set; }
        public int? typeActivity { get; set; }
        public bool? status { get; set; }
    }

}