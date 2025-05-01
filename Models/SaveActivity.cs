using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.Models
{
    public class SaveActivity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public int ActiviteId { get; set; }

        [Required]
        public Activite Activite { get; set; }

        public bool isFavorite { get; set; } 
        public bool isToLater { get; set; } 

    }
}
