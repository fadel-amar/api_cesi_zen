using CesiZen_API.DTO;
using CesiZen_API.Models;

namespace CesiZen_API.Services.Interfaces
{
    public interface IActivityService
    {
        Task<IEnumerable<Activite>> GetAllActivities(bool isAdmin);
        Task<Activite> GetActivityById(int id, bool isAdmin);
        Task<IEnumerable<Activite>> GetAllActivitiesFavorite(User user);
        Task<IEnumerable<Activite>> GetAllActivitiesSaveToLater(User user);
        Task<Activite> CreateActivity(CreateActivityDTO createActivityDTO, User user);
        Task<Activite> UpdateActivity(int id, UpdateActivityDTO updateActivityDTO, User user);
        Task<Boolean> DeleteActivity(int id);
        Task<Boolean> ToggleFavorite(User user, int activityId);
        Task<Boolean> ToggleToLater(User user, int activityId);
    }
}
