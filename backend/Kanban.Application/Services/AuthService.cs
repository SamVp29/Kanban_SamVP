using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Kanban.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kanban.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Correo);
        if (usuario == null) return null;

        if (!_passwordHasher.VerifyPassword(dto.Password, usuario.PasswordHash))
        {
            return null; // Contraseña incorrecta
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "super_secret_key_de_evaluacion_12345");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Name, usuario.Nombre)
            }),
            Expires = DateTime.UtcNow.AddHours(4),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            UsuarioId = usuario.Id,
            Nombre = usuario.Nombre
        };
    }
}
