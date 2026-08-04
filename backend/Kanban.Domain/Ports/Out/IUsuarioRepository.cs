using Kanban.Domain.Entities;

namespace Kanban.Domain.Ports.Out;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
}
