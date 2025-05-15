using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CesiZen_API.Helper;


namespace CesiZen_API.Models
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(25)]
        [Required]
        public string Login { get; set; }

        [MaxLength(150)]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }


        [Required]
        [MaxLength(10)]
        public string Role { get; set; } = Constants.ROLE_USER;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool Disabled { get; set; } = false;

        public bool Banned { get; set; } = false;

        public void SetPassword(string plainPassword)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            Password = Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string plainPassword)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            var hashedPassword = Convert.ToBase64String(hashedBytes);
            return Password == hashedPassword;
        }
    }

}