using Kanban.Domain.Entities;
using Kanban.Domain.Interfaces;
using Kanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class ProyectoRepository : Repository<Proyecto>, IProyectoRepository
{
    public ProyectoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Proyecto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? nombreFiltro)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombreFiltro))
        {
            query = query.Where(p => p.Nombre.ToLower().Contains(nombreFiltro.ToLower()));
        }

        int totalCount = await query.CountAsync();

        var items = await query.OrderByDescending(p => p.FechaInicio)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return (items, totalCount);
    }
}
