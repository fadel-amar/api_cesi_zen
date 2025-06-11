using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace CesiZen_API.ModelBlinders
{
    public class CurrentUserModelBinder : IModelBinder
    {
        private readonly IUserService _userService;

        public CurrentUserModelBinder(IUserService userService)
        {
            _userService = userService;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var userClaims = bindingContext.HttpContext.User;

            if (!userClaims.Identity?.IsAuthenticated ?? true)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var userIdClaim = userClaims.FindFirst("UserId")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(user);
        }
    }
}