using CesiZen_API.Models.DTO;
using CesiZen_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public AuthController(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDTO.RegisterDTO userDto)
    {
        var errors = new List<string>();

        if (await _context.User.AnyAsync(u => u.Email == userDto.Email))
            errors.Add("L'email est déjà utilisé");

        if (await _context.User.AnyAsync(u => u.Login == userDto.Login))
            errors.Add("Le login est déjà utilisé");

        if (errors.Count > 0)
        {
            return BadRequest(new
            {
                status = 400,
                errors = errors
            });
        }

        var user = new User
        {
            Email = userDto.Email,
            Login = userDto.Login,
            Password = HashPassword(userDto.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new
        {
            status = 201,
            message = "Utilisateur créé avec succès"
        });
    }




    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO.LoginDTO loginDto)
    {


        if (string.IsNullOrWhiteSpace(loginDto.Email) && string.IsNullOrWhiteSpace(loginDto.Login))
        {
            return BadRequest(new
            {
                status = 400,
                message = "Vous devez fournir soit un email, soit un login."
            });
        }

        var existingUser = await _context.User.FirstOrDefaultAsync(u => (u.Email == loginDto.Email) || u.Login ==loginDto.Login );

        if (existingUser == null || existingUser.Password != HashPassword(loginDto.Password))

            return Unauthorized(
            new
            {
                staus = 401,
                message = "Identifiants invalides"
            });


        var token = _authService.GenerateJwtToken(existingUser);
        return Ok(new { Token = token });
    }


    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
