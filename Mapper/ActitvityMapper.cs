using CesiZen_API.DTO.Response;
using CesiZen_API.Models;

namespace CesiZen_API.Mapper
{
    public static class ActitvityMapper
    {
        public static FullActivityResponseDTO toResponseFullDTO(Activite activity, int? userId = null)
        {
            SaveActivity? save = null;
            if (userId != null)
            {
                save = activity.SavedActivities.FirstOrDefault(s => s.UserId == userId);

            }

            return new FullActivityResponseDTO
            {
                Id = activity.Id,
                Title = activity.Title,
                ImagePresentation = activity.ImagePresentation,
                Description = activity.Description,
                Url = activity.Url,
                Status = activity.Status,
                Type = activity.TypeActitvity,
                DurationMin = activity.Duree,
                Category = activity.Category.Name,
                User = activity.User.Id,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt,
                IsFavorite = save?.IsFavorite ?? false,
                IsToLater = save?.IsToLater ?? false
            };
        }

        public static ActivityResponseDTO toResponseDTO(Activite activity)
        {
            return new ActivityResponseDTO
            {
                Id = activity.Id,
                Title = activity.Title,
                ImagePresentation = activity.ImagePresentation,
                Type = activity.TypeActitvity,
                Status = activity.Status,
                Category = activity.Category.Name
            };
        }
        public static ListActivityResponseDTO toListResponseDTO(IEnumerable<Activite> activities)
        {
            return new ListActivityResponseDTO
            {
                Activities = activities.Select(toResponseDTO).ToList()
            };

        }
    }
}
