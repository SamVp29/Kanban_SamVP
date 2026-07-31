using Kanban.Domain.Entities;

namespace Kanban.Domain.Interfaces;

public interface ITareaRepository : IRepository<Tarea>
{
    Task<IEnumerable<Tarea>> GetByColumnaIdAsync(int columnaId);
}
