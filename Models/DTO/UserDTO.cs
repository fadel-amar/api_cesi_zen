using System.ComponentModel.DataAnnotations;

namespace CesiZen_API.Models.DTO
{
    public class UserDTO
    {
        public class LoginDTO
        {
          
            public string Email { get; set; }
     
            public string Login { get; set; }

            [Required(ErrorMessage = "Le mot de passe est obligatoire")]
            public string Password { get; set; }
        }


        public class RegisterDTO
        {

            [Required(ErrorMessage = "L'email est obligatoire")]
            [EmailAddress(ErrorMessage = "L'email doit être valide")]

            public string Email { get; set; }
            [Required(ErrorMessage = "L'identifiant doit être renseigné")]
            public string Login { get; set; }

            [Required(ErrorMessage = "Le mot de passe est obligatoire")]
            public string Password { get; set; }

        }
    }
}
