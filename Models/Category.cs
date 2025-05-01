using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CesiZen_API.Models
{
    public class Category
    {

        public int Id { get; set; }

        [MaxLength(25)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool Visibility { get; set; } = true;
        public ICollection<Activite> Activites { get; set; } = new List<Activite>();
        public User User { get; set; }


    }
}