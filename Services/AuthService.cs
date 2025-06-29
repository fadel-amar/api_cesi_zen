using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CesiZen_API.DTO;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;


namespace CesiZen_API.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AuthService(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }
        public async Task<string> Register(RegisterDTO newUser)
        {
            User user = new User();
            user.Login = newUser.Login;
            user.Email = newUser.Email;
            user.SetPassword(newUser.Password);
            User userCreated = await _userService.CreateUser(user);
            if (userCreated == null)
            {
                throw new Exception("Erreur lors de la création de l'utilisateur");
            }
            return GenerateJwtToken(user);
        }

        public async Task<string?> Login(LoginDTO loginDto)
        {
            User? existing = await _userService.GetUserByIdentifier(loginDto.Identifier);

            if (existing == null || !existing.VerifyPassword(loginDto.Password))
            {
                throw new UnauthorizedAccessException("Identifiants invalides");
            }
            if(existing.Banned)
            {
                throw new UnauthorizedAccessException("Votre compte à été banni");
            }
            if(existing.Disabled)
            {
                throw new UnauthorizedAccessException("Votre compte à été désactiver Veuillez conctacter le service CesiZen");
            }

            return GenerateJwtToken(existing);
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserId", user.Id.ToString()),
            new Claim("role", user.Role)
        };

            var key = Environment.GetEnvironmentVariable("JWT_KEY");

            if (string.IsNullOrEmpty(key))
                throw new Exception("JWT_KEY n'est pas défini dans l'environnement.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

         }

}