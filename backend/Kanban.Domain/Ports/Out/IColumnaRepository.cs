using Kanban.Domain.Entities;

namespace Kanban.Domain.Ports.Out;

public interface IColumnaRepository : IRepository<Columna>
{
    Task<IEnumerable<Columna>> GetByProyectoIdAsync(int proyectoId);
}
