using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.ModelBlinders
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CurrentUserAttribute : ModelBinderAttribute
    {
        public CurrentUserAttribute() : base(typeof(CurrentUserModelBinder)) { }
    }
}
