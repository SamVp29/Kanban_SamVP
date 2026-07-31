using Kanban.Domain.Entities;

namespace Kanban.Domain.Interfaces;

public interface IProyectoRepository : IRepository<Proyecto>
{
    Task<(IEnumerable<Proyecto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? nombreFiltro);
}
