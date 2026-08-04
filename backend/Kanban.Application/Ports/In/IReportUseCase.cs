namespace Kanban.Application.Ports.In;

public interface IReportUseCase
{
    Task<byte[]> GenerateReportAsync(int proyectoId, string format, string? prioridad = null, int? responsableId = null, string? texto = null);
}
