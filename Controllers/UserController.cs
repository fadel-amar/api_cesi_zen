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
        public async Task<IActionResult> GetAllUsers()
        {
            var users =await _context.User
                .Select(u => new { u.Id, u.Login, u.Email, u.CreatedAt, u.UpdatedAt, u.Banned, u.Disabled })
                .ToListAsync();

            if (users != null)
            {
                return Ok(users);
            }

            return NotFound(
             new
             {
                 staus = 404,
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
                 staus = 404,
                 message = "Aucun utilisateur a été trouvé"
             });

        }
    }

}
