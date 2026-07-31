using Kanban.Domain.Entities;
using Kanban.Domain.Interfaces;
using Kanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class TareaRepository : Repository<Tarea>, ITareaRepository
{
    public TareaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tarea>> GetByColumnaIdAsync(int columnaId)
    {
        return await _dbSet.Where(t => t.ColumnaId == columnaId)
                           .OrderBy(t => t.Orden)
                           .ToListAsync();
    }
}
