using Kanban.Application.DTOs;

namespace Kanban.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
    Task<IEnumerable<UsuarioResponseDto>> GetAllUsersAsync();
}
