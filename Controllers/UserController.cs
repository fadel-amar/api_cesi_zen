using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CesiZen_API.Controllers
{


    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "AppDbContext n'est pas défini");
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {
            var query = _context.User.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.Login.Contains(filter) || u.Email.Contains(filter));
            }

            var totalUsers = await query.CountAsync();
            var users = await query
                .Select(u => new { u.Id, u.Login, u.Email, u.CreatedAt, u.UpdatedAt, u.Banned, u.Disabled })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (users != null)
            {
                return Ok(new
                {
                    TotalUsers = totalUsers,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Users = users
                });
            }

            return NotFound(
             new
             {
                 status = 404,
                 message = "Aucun utilisateur a été trouvé"
             });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.User
                .Where(u => u.Id == id)
                .Select(u => new { u.Id, u.Login, u.Email, u.CreatedAt, u.UpdatedAt, u.Banned, u.Disabled })
                .FirstOrDefaultAsync();

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound(
             new
             {
                 status = 404,
                 message = "Aucun utilisateur a été trouvé"
             });
        }
    }

}
