using System.ComponentModel.DataAnnotations;


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
        public required string Email { get; set; }

        [Required(ErrorMessage = "L'identifiant doit être renseigné")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        public required string Password { get; set; }

    }

    public class PatchDTO
    {
        public string? Login { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool? Disabled { get; set; }
        public bool? Banned { get; set; }
    }
}
