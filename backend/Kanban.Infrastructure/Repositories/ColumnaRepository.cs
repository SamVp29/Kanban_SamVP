using Kanban.Domain.Entities;
using Kanban.Domain.Ports.Out;
using Kanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class ColumnaRepository : Repository<Columna>, IColumnaRepository
{
    public ColumnaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Columna>> GetByProyectoIdAsync(int proyectoId)
    {
        return await _dbSet.Where(c => c.ProyectoId == proyectoId)
                           .OrderBy(c => c.Orden)
                           .ToListAsync();
    }
}
