using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CesiZen_API.Models
{
    public class Page
    {
        public int Id { get; set; }

        [MaxLength(35)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public bool Visibility { get; set; } = true;

        public Menu Menu { get; set; }

        public User User { get; set; }
    }
}
