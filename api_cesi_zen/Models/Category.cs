using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.Models
{
    public class Category
    {

        public int Id { get; set; }

        [MaxLength(25)]
        public required string Name { get; set; }
        public required string Emoji { get; set; }
        public required string Duration { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public required bool Visibility { get; set; } = false;
        public ICollection<Activite> Activites { get; set; }
        public required User User { get; set; }


    }
}