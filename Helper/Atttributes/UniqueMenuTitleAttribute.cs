using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using CesiZen_API.Services.Interfaces;
using CesiZen_API.Models;

namespace CesiZen_API.Helper.Attributes
{
    public class UniqueMenuTitleAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var title = value as string;

            if (string.IsNullOrWhiteSpace(title))
                return ValidationResult.Success;

            IMenuService menuService = validationContext.GetService(typeof(IMenuService)) as IMenuService;

            if (menuService == null)
                throw new InvalidOperationException("IMenuService n'est pas disponible dans le conteneur DI.");

            Menu? menuExisting = menuService.GetMenuByTitle(title).Result;

            if (menuExisting != null)
            {
                return new ValidationResult("Ce titre de menu est déjà utilisé.");
            }

            return ValidationResult.Success;
        }
    }
}
