using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CesiZen_API.Models
{
    public class Page
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(255)]
        public string? link { get; set; }

        [MaxLength(50)]
        public string? type_link { get; set; }

        public bool Visibility { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Menu? Menu { get; set; }

        public User User { get; set; }
    }
}
