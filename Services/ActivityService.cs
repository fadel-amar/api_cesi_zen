using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using CesiZen_API.DTO;
using CesiZen_API.Helper;
using CesiZen_API.Helper.Exceptions;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_API.Services
{
    public class ActivityService : IActivityService
    {

        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;

        public ActivityService(AppDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }
        public async Task<IEnumerable<Activite>> GetAllActivities(bool isAdmin, FilterActivity? filter)
        {
            var query = _context.Activite.Include(p => p.Category).AsQueryable();
            if (!isAdmin)
            {
                query = query.Where(p => p.Status == true);
            }

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.title))
                    query = query.Where(p => p.Title.Contains(filter.title));

                if (filter.category.HasValue)
                    query = query.Where(p => p.Category.Id == filter.category.Value);

                if (filter.typeActivity.HasValue)
                    query = query.Where(p => p.TypeActitvity == Constants.ACTIVITY_TYPES[filter.typeActivity.Value]);

                if (isAdmin && filter.status.HasValue)
                    query = query.Where(p => p.Status == filter.status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Activite> GetActivityById(int id, bool isAdmin)
        {
            var query = _context.Activite
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.SavedActivities)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(p => p.Status);
            }

            Activite? activity = await query.FirstOrDefaultAsync(p => p.Id == id);

            if (activity == null)
            {
                throw new NotFoundException("Cette activité n'a pas été trouvée");
            }
            return activity;
        }
        public async Task<IEnumerable<Activite>> GetAllActivitiesFavorite(User user)
        {
            var saveActivities = await _context.SaveActivity
                .Include(sa => sa.Activite)
                .ThenInclude(a => a.Category)
                .Where(sa => sa.UserId == user.Id && sa.IsFavorite)
                .ToListAsync();

            var activities = saveActivities
                .Select(sa => sa.Activite)
                .ToList();

            if (!activities.Any())
            {
                throw new NotFoundException("Aucune activité favorite trouvée pour cet utilisateur");
            }

            return activities;
        }

        public async Task<IEnumerable<Activite>> GetAllActivitiesSaveToLater(User user)
        {
            var saveActivities = await _context.SaveActivity
                .Include(sa => sa.Activite)
                .ThenInclude(a => a.Category)
                .Where(sa => sa.UserId == user.Id && sa.IsToLater)
                .ToListAsync();

            var activities = saveActivities
                .Select(sa => sa.Activite)
                .ToList();

            if (!activities.Any())
            {
                throw new NotFoundException("Aucune activité enregistrer pour plus tard trouvée");
            }

            return activities;
        }

        public async Task<Activite> CreateActivity(CreateActivityDTO createActivityDTO, User user)
        {
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var allowedMediaExtensions = new[] { ".mp4", ".mp3", ".wav" };

            if (createActivityDTO == null)
            {
                throw new BadRequestException("L'activité est invalide");
            }
            Category? category = await _categoryService.GetCategoryById(createActivityDTO.CategoryId, true);
            if(category.Visibility == false )
            {
                throw new BadRequestException("Vous ne pouvez pas assigner cette catégorie à cette activité car elle est en status non visible");
            }


            // Vérifier les extensions
            var imageExt = Path.GetExtension(createActivityDTO.ImagePresentation.FileName).ToLower();
            var mediaExt = Path.GetExtension(createActivityDTO.Url.FileName).ToLower();

            if (!allowedImageExtensions.Contains(imageExt))
                throw new BadRequestException("Format d'image non supporté.");

            if (!allowedMediaExtensions.Contains(mediaExt))
                throw new BadRequestException("Format média non supporté (vidéo ou audio seulement).");

            // Sauvegarder l'image
            var imageName = $"{Guid.NewGuid()}{imageExt}";
            var imagePath = Path.Combine("wwwroot/images", imageName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
                await createActivityDTO.ImagePresentation.CopyToAsync(stream);

            // Sauvegarder le média (vidéo/musique)
            var mediaName = $"{Guid.NewGuid()}{mediaExt}";
            var mediaPath = Path.Combine("wwwroot/medias", mediaName);
            using (var stream = new FileStream(mediaPath, FileMode.Create))
                await createActivityDTO.Url.CopyToAsync(stream);
            Activite activity = new Activite
            {
                Title = createActivityDTO.Title,
                Description = createActivityDTO.Description,
                ImagePresentation = $"/images/{imageName}",
                Url = $"/medias/{mediaName}",
                User = user,
                Category = category,
                Duree = createActivityDTO.DurationMin,
                TypeActitvity = Constants.ACTIVITY_TYPES[createActivityDTO.TypeActivitty]
            };
            _context.Activite.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<Activite> UpdateActivity(int id, UpdateActivityDTO updateActivityDTO, User user)
        {
            if (updateActivityDTO == null)
            {
                throw new BadRequestException("Aucune donnée à mettre à jour");
            }

            Category? category = null;
            if (updateActivityDTO.CategoryId.HasValue)
            {
                category = await _categoryService.GetCategoryById(updateActivityDTO.CategoryId.Value, true);
            }

            Activite? activityExisting = await GetActivityById(id, true);
            activityExisting.Status = updateActivityDTO.Status ?? activityExisting.Status;
            activityExisting.Title = updateActivityDTO.Title ?? activityExisting.Title;
            activityExisting.Description = updateActivityDTO.Description ?? activityExisting.Description;
            activityExisting.Duree = updateActivityDTO.DurationMin ?? activityExisting.Duree;
            activityExisting.TypeActitvity = updateActivityDTO.TypeActivitty.HasValue
                ? Constants.ACTIVITY_TYPES[updateActivityDTO.TypeActivitty.Value]
                : activityExisting.TypeActitvity;
            activityExisting.Category = category ?? activityExisting.Category;
            activityExisting.User = user;

            // Fichier vidéo
            if (updateActivityDTO.Url != null)
            {
                // Supprimer ancien fichier vidéo
                if (!string.IsNullOrWhiteSpace(activityExisting.Url))
                {
                    var oldVideoPath = Path.Combine("wwwroot/medias", Path.GetFileName(activityExisting.Url));
                    if (File.Exists(oldVideoPath))
                        File.Delete(oldVideoPath);
                }

                var videoExt = Path.GetExtension(updateActivityDTO.Url.FileName);
                var newVideoName = $"{Guid.NewGuid()}{videoExt}";
                var videoPath = Path.Combine("wwwroot/medias", newVideoName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await updateActivityDTO.Url.CopyToAsync(stream);
                }

                activityExisting.Url = $"/medias/{newVideoName}";
            }

            // Fichier image
            if (updateActivityDTO.ImagePresentation != null)
            {
                // Supprimer ancienne image
                if (!string.IsNullOrWhiteSpace(activityExisting.ImagePresentation))
                {
                    var oldImagePath = Path.Combine("wwwroot/images", Path.GetFileName(activityExisting.ImagePresentation));
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                var imageExt = Path.GetExtension(updateActivityDTO.ImagePresentation.FileName);
                var newImageName = $"{Guid.NewGuid()}{imageExt}";
                var imagePath = Path.Combine("wwwroot/images", newImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await updateActivityDTO.ImagePresentation.CopyToAsync(stream);
                }

                activityExisting.ImagePresentation = $"/images/{newImageName}";
            }

            _context.Activite.Update(activityExisting);
            await _context.SaveChangesAsync();
            return activityExisting;
        }


        public async Task<Boolean> DeleteActivity(int id)
        {
            Activite activity = await GetActivityById(id, true);
            if (!string.IsNullOrWhiteSpace(activity.Url))
            {
                var mediaPath = Path.Combine("wwwroot/medias", Path.GetFileName(activity.Url));
                if (File.Exists(mediaPath))
                    File.Delete(mediaPath);
            }

            if (!string.IsNullOrWhiteSpace(activity.ImagePresentation))
            {
                var imagePath = Path.Combine("wwwroot/images", Path.GetFileName(activity.ImagePresentation));
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            _context.Activite.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ToggleFavorite(User user, int activityId)
        {
            var existing = await _context.SaveActivity
                .FirstOrDefaultAsync(sa => sa.User.Id == user.Id && sa.Activite.Id == activityId);

            if (existing == null)
            {
                var save = new SaveActivity
                {
                    UserId = user.Id,
                    ActiviteId = activityId,
                    IsFavorite = true,
                };
                _context.SaveActivity.Add(save);
            }
            else
            {
                existing.IsFavorite = !existing.IsFavorite;
                _context.SaveActivity.Update(existing);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleToLater(User user, int activityId)
        {
            var existing = await _context.SaveActivity
                .FirstOrDefaultAsync(sa => sa.User.Id == user.Id && sa.Activite.Id == activityId);

            if (existing == null)
            {
                var activity = await GetActivityById(activityId, false);

                var save = new SaveActivity
                {
                    User = user,
                    Activite = activity,
                    IsToLater = true,
                    IsFavorite = false,
                };

                _context.SaveActivity.Add(save);
            }
            else
            {
                existing.IsToLater = !existing.IsToLater;
                _context.SaveActivity.Update(existing);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Activite>> GetTopActivities()
        {
            var topActivities = await _context.Activite
                .Include(sa => sa.Category)
                .Join(_context.SaveActivity,
                      a => a.Id,
                      sa => sa.ActiviteId,
                      (a, sa) => new { Activity = a, SaveActivity = sa })
                .GroupBy(x => x.Activity.Id)
                .Select(g => new
                {
                    ActivityId = g.Key,
                    SaveCount = g.Count(),
                    ActivityDetails = g.Select(x => x.Activity)
                })
                .OrderByDescending(x => x.SaveCount)
                .Take(3)
                .Select(x => x.ActivityDetails.FirstOrDefault()!) 
                .ToListAsync();

            var query = _context.Activite.Include(p => p.Category).AsQueryable();
            query = query.Where(p => p.Status);
            if (!topActivities.Any())
            {
                topActivities = await _context.Activite
                   .Include(p => p.Category)
                   .Where(p => p.Status)
                   .OrderBy(p => p.Id)
                   .Take(3)
                   .ToListAsync();
            }

            return topActivities;
        }
    }
}
