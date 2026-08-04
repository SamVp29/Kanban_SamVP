using Kanban.Domain.Entities;

namespace Kanban.Domain.Ports.Out;

public interface IProyectoRepository : IRepository<Proyecto>
{
    Task<IEnumerable<Proyecto>> GetPagedAsync(int pageNumber, int pageSize, string? search);
    Task<int> GetTotalCountAsync(string? search);
    Task<Proyecto?> GetProyectoCompletoReporteAsync(int proyectoId);
}
