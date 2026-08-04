using Kanban.Application.DTOs;

namespace Kanban.Application.Ports.In;

public interface IAuthUseCase
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<IEnumerable<UsuarioResponseDto>> GetAllUsersAsync();
}
