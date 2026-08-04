using Kanban.Domain.Entities;

namespace Kanban.Domain.Ports.Out;

public interface ITareaRepository : IRepository<Tarea>
{
    Task<IEnumerable<Tarea>> GetByColumnaIdAsync(int columnaId);
}
