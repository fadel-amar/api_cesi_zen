using System.ComponentModel.DataAnnotations;
using CesiZen_API.Models;

namespace CesiZen_API.Models
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(35)]
        [Required]
        public string Title { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Menu? Parent { get; set; }
        public ICollection<Menu> SousMenus { get; set; } = new List<Menu>();
        public ICollection<Page> Pages { get; set; } = new List<Page>();
        public User User { get; set; }

    }
}
