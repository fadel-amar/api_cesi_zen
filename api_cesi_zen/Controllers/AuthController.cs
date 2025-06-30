using CesiZen_API.DTO;
using CesiZen_API.Models;
using CesiZen_API.Services;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Register([FromBody] RegisterDTO userDto)
    {
        string? result = await _authService.Register(userDto);

        return Ok(new
        {
            token = result
        });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {

        String? result = await _authService.Login(loginDto);

        if (result == null)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "Identifiants invalides"
            });
        }

        return Ok(new
        {
            token = result
        });

    }
}
