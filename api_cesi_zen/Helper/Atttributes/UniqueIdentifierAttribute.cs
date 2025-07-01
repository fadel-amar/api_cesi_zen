using System.ComponentModel.DataAnnotations;
using CesiZen_API.Services.Interfaces;
using CesiZen_API.Models;

namespace CesiZen_API.Helper.Atttributes
{
    public class UniqueIdentifierAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var identifier = value as string;
            if (string.IsNullOrWhiteSpace(identifier))
                return ValidationResult.Success;

            var userService = validationContext.GetService(typeof(IUserService)) as IUserService;

            if (userService == null)
                return new ValidationResult("Impossible de valider l'unicité de l'email.");


            User? userExists = userService.GetUserByIdentifier(identifier).Result;
            if (userExists != null)
                return new ValidationResult(ErrorMessage ?? "Cet identifiant est déjà utilisé.");

            return ValidationResult.Success;
        }
    }

}