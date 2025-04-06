using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class User
{

    public int Id { get; set; }

    [Required(ErrorMessage = "L'identifiant doit être renseigné")]
    [MaxLength(25, ErrorMessage = "Le login ne doit pas dépasser 25 caractères")]
    public string Login { get; set; }

    [Required(ErrorMessage = "L'email doit être renseigné")]
    [EmailAddress(ErrorMessage = "L'email doit être valide")]
    [MaxLength(150, ErrorMessage = "L'email ne doit pas dépasser 150 caractères")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{6,}$", 
      ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères, une majuscule, une minuscule et un chiffre")]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; } = "User";

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool Disabled { get; set; } = false;

    public bool Banned { get; set; } = false;
}
