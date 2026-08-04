using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Kanban.Domain.Interfaces;

namespace Kanban.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Correo);
        if (usuario == null) return null;

        if (!_passwordHasher.VerifyPassword(dto.Password, usuario.PasswordHash))
        {
            return null; // Contraseña incorrecta
        }

        var token = _jwtTokenGenerator.GenerateToken(usuario.Id, usuario.Correo, usuario.Nombre);

        return new AuthResponseDto
        {
            Token = token,
            UsuarioId = usuario.Id,
            Nombre = usuario.Nombre
        };
    }

    public async Task<IEnumerable<UsuarioResponseDto>> GetAllUsersAsync()
    {
        var users = await _usuarioRepository.GetAllAsync();
        return users.Select(u => new UsuarioResponseDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Correo = u.Correo
        });
    }
}
