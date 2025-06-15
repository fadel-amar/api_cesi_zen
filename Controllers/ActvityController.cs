using System.Security.Claims;
using CesiZen_API.DTO;
using CesiZen_API.DTO.Response;
using CesiZen_API.Mapper;
using CesiZen_API.ModelBlinders;
using CesiZen_API.Models;
using CesiZen_API.Services;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{
    [Route("api/activities")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            bool isAdmin = User.IsInRole("Admin");
            IEnumerable<Activite>? activities = await _activityService.GetAllActivities(isAdmin);
            return Ok(ActitvityMapper.toListResponseDTO(activities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            bool isAdmin = User.IsInRole("Admin");
            Activite? activity = await _activityService.GetActivityById(id, isAdmin);
            return Ok(ActitvityMapper.toResponseFullDTO(activity));
        }

        [HttpGet("favorite")]
        [Authorize()]
        public async Task<IActionResult> GetActivitiesFavorite([CurrentUser] User user)
        {
            IEnumerable<Activite> activitiesFavorite = await _activityService.GetAllActivitiesFavorite(user);
            return Ok(ActitvityMapper.toListResponseDTO(activitiesFavorite));
        }

        [HttpGet("toLater")]
        [Authorize()]
        public async Task<IActionResult> GetAllActivitiesSaveToLater([CurrentUser] User user)
        {
            IEnumerable<Activite> activitiesFavorite = await _activityService.GetAllActivitiesSaveToLater(user);
            return Ok(ActitvityMapper.toListResponseDTO(activitiesFavorite));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateActivty([FromForm] CreateActivityDTO createActivityDTO, [CurrentUser] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Erreur de validation",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            Activite activity = await _activityService.CreateActivity(createActivityDTO, user);
            return StatusCode(201, new
            {
                status = 201,
                message = "L'activité a bien été créé",
                data = ActitvityMapper.toResponseFullDTO(activity)
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateActivity(int id, [FromForm] UpdateActivityDTO updateActivityDTO, [CurrentUser] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Erreur de validation",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }
            Activite activity = await _activityService.UpdateActivity(id, updateActivityDTO, user);
            return Ok(ActitvityMapper.toResponseFullDTO(activity));
        }

        [HttpPost("favorites/{activityId}")]
        [Authorize()]
        public async Task<IActionResult> ToggleFavorite(int activityId, [CurrentUser] User user)
        {
            bool saveActivity = await _activityService.ToggleFavorite(user, activityId);
            return Ok();

        }

        [HttpPost("tolater/{activityId}")]
        [Authorize]
        public async Task<IActionResult> ToggleToLater(int activityId, [CurrentUser] User user)
        {

            bool saveActivity = await _activityService.ToggleToLater(user, activityId);
            return Ok();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            await _activityService.DeleteActivity(id);
            return StatusCode(204, new
            {
                status = 204,
                message = "L'activité a bien été supprimée"
            });
        }

    }
}
