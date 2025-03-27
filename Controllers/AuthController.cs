using CesiZen_API.Models;
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
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (await _context.User.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest(new
            {
                status = 400,
                message = "Email dèjà utiilisé"
            });
        }

        user.PasswordHash = HashPassword(user.PasswordHash);
        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new
        {
            status = 201,
            message = "Utilisateur créer avex susccés",
            data = new
            {
                id = user.Id,
                email = user.Email
            }
        });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (existingUser == null || existingUser.PasswordHash != HashPassword(loginDto.Password))
            return Unauthorized("Invalid email or password.");

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
