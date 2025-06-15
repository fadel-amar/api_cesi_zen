using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.Models
{
    public class Activite
    {
        public int Id { get; set; }

        [MaxLength(30)]
        [Required]
        public required string Title { get; set; }

        [Required]
        [Range(1, 120)] // Durée en minutes, max = 2h
        public int Duree { get; set; }

        [MaxLength(255)]
        [Required]
        public required string Description { get; set; }

        [MaxLength(50)]
        [Required]
        public required string TypeActitvity { get; set; }

        [MaxLength(2083)]
        [Required]
        [Url]
        public required string ImagePresentation { get; set; }

        [MaxLength(2083)]
        [Required]
        [Url]
        public required string Url { get; set; }

        public Boolean Status { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public required Category Category { get; set; }

        [Required]
        public required User User { get; set; }
    }
}
