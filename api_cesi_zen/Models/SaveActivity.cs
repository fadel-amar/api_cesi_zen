using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.Models
{
    public class SaveActivity
    {
        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public int ActiviteId { get; set; }

        public Activite Activite { get; set; }

        public bool IsFavorite { get; set; } = false;
        public bool IsToLater { get; set; } = false;
    }
}
