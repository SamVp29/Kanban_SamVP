using Kanban.Domain.Entities;
using Kanban.Domain.Ports.Out;
using Kanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class ProyectoRepository : Repository<Proyecto>, IProyectoRepository
{
    public ProyectoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Proyecto>> GetPagedAsync(int pageNumber, int pageSize, string? search)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Nombre.ToLower().Contains(search.ToLower()));
        }

        return await query.OrderByDescending(p => p.FechaInicio)
                          .Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? search)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Nombre.ToLower().Contains(search.ToLower()));
        }

        return await query.CountAsync();
    }

    public async Task<Proyecto?> GetProyectoCompletoReporteAsync(int proyectoId)
    {
        return await _dbSet
            .Include(p => p.Columnas)
                .ThenInclude(c => c.Tareas)
                    .ThenInclude(t => t.Responsable)
            .FirstOrDefaultAsync(p => p.Id == proyectoId);
    }
}
