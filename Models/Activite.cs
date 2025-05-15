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
        public TimeSpan Duree { get; set; }

        [MaxLength(255)]
        [Required]
        public required string Description { get; set; }

        [Required]
        public required string TypeActitvity { get; set; }

        [Required]
        public required string url { get; set; }

        public required int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public required Category Category { get; set; }

        [Required]
        public required User User { get; set; }
    }
}

