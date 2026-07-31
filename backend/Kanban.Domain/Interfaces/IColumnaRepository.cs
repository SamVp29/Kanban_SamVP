using Kanban.Domain.Entities;

namespace Kanban.Domain.Interfaces;

public interface IColumnaRepository : IRepository<Columna>
{
    Task<IEnumerable<Columna>> GetByProyectoIdAsync(int proyectoId);
}
