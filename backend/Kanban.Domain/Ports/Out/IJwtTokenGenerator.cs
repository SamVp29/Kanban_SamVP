namespace Kanban.Domain.Ports.Out;

public interface IJwtTokenGenerator
{
    string GenerateToken(int usuarioId, string correo, string nombre);
}
