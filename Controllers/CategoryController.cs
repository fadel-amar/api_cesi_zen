using CesiZen_API.DTO;
using CesiZen_API.Mapper;
using CesiZen_API.ModelBlinders;
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
            bool isAdmin = User.IsInRole("Admin");
            IEnumerable<Category>? categories = await _categoryService.GetAllCategories(isAdmin);
            return Ok(new { categories = CategoryMapper.ToResponseListDto(categories) });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            bool isAdmin = User.IsInRole("Admin");
            Category category = await _categoryService.GetCategoryById(id, isAdmin);
            return Ok(CategoryMapper.ToResponseFullDto(category));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto, [CurrentUser] User user)
        {
            var created = await _categoryService.CreateCategory(user, categoryDto);
            return StatusCode(201, new
            {
                status = 201,
                message = "Votre catégorie a bien été créé"
            });

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto, [CurrentUser] User user)
        {
            bool updated = await _categoryService.UpdateCategory(id, user, categoryDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _categoryService.DeleteCategory(id);
            return NoContent();
        }
    }
}