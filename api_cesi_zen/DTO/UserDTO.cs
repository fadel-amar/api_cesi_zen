using System.ComponentModel.DataAnnotations;
using CesiZen_API.Helper.Atttributes;


namespace CesiZen_API.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "L'identifiant est obligatoire")]
        public required string Identifier { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        public required string Password { get; set; }
    }


    public class RegisterDTO
    {

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "L'email doit être valide")]
        [UniqueIdentifier(ErrorMessage = "L'email est dèjà utilisé")]
        [MaxLength(150, ErrorMessage = "L'email doit avoir moins de 150 caractères")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "L'identifiant doit être renseigné")]
        [UniqueIdentifier(ErrorMessage = "Ce login est déjà utilisé")]
        [MaxLength(25, ErrorMessage = "Le login doit avoir moins de 25 caractères")]
        [MinLength(3 , ErrorMessage = "Le login doit avoir au moins 3 caractères")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères, incluant des lettres, des chiffres et des caractères spéciaux")]
        public required string Password { get; set; }

    }

    public class PatchDTO
    {
        [MaxLength(25, ErrorMessage = "Le login doit avoir moins de 25 carctères")]
        [MinLength(3, ErrorMessage = "Le login doit avoir au moins 3 caractères")]
        [UniqueIdentifier(ErrorMessage = "Ce login est déjà utilisé")]
        public string? Login { get; set; }
        [MaxLength(150, ErrorMessage ="L'email doit avoir moins de 150 caractères")]
        [UniqueIdentifier(ErrorMessage = "Cet email est déjà utilisé")]
        public string? Email { get; set; }
        [RegularExpression(@"^(User|Admin)$", ErrorMessage = "Le rôle doit être soit 'User' soit 'Admin'")]
        public string? Role { get; set; }
        public bool? Disabled { get; set; }
        public bool? Banned { get; set; }
    }

    public class UpdateMyAccontDTO
    {
        [MaxLength(25, ErrorMessage = "Le login doit avoir moins de 25 carctères")]
        [MinLength(3, ErrorMessage = "Le login doit avoir au moins 3 caractères")]
        [UniqueIdentifier(ErrorMessage = "Ce login est déjà utilisé")]
        public string? Login { get; set; }
        [UniqueIdentifier(ErrorMessage = "Cet email est déjà utilisé")]
        [MaxLength(150, ErrorMessage = "L'email doit avoir moins de 150 caractères")]
        public string? Email { get; set; }
        public bool? Disabled { get; set; }
    }

    public class ResetMyPasswordDTO
    {
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères, incluant des lettres, des chiffres et des caractères spéciaux")]
        public required string NewPassword { get; set; }
    }
}
