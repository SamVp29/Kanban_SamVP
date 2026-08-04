namespace Kanban.Application.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(int usuarioId, string correo, string nombre);
}
