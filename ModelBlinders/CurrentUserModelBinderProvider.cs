using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CesiZen_API.ModelBlinders
{
    public class CurrentUserModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(User))
            {
                var userService = context.Services.GetRequiredService<IUserService>();
                return new CurrentUserModelBinder(userService);
            }

            return null;
        }
    }
}
