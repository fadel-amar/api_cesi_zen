using CesiZen_API.DTO;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            IEnumerable<Category>? categories = await _categoryService.GetAllCategories();
            if (categories == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Aucune catégorie a été trouvé"
                });
            }

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            Category? category = await _categoryService.GetCategoryById(id);
            if (category == null)
                return NotFound(new
                {
                    status = 404,
                    message = "Ce catégorie n'a pas été trouvé"
                });

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO.CreateCategoryDto categoryDto)
        {
            Category category = new Category { Name = categoryDto.Name };
            var created = await _categoryService.CreateCategory(category);
            return StatusCode(201, new
            {
                status = 201,
                message = "Votre catégorie a bien été créé"
            });

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO.UpdateCategoryDto categoryDto)
        {
            Category? category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Ce catégorie n'a pas été trouvé"
                });
            }

            category.Name = categoryDto.Name;
            bool updated = await _categoryService.UpdateCategory(category);
            if (!updated)
                return BadRequest(new { status = 400, message = $"Impossible de mettre à jour. Catégorie {id} introuvable." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _categoryService.DeleteCategory(id);
            if (!deleted)
                return NotFound(new { status = 404, message = $"Impossible de supprimer. Catégorie introuvable." });

            return NoContent();
        }
    }
}