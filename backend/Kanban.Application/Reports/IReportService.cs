using Kanban.Application.DTOs;

namespace Kanban.Application.Reports;

public interface IReportService
{
    Task<byte[]> GenerateReportAsync(int proyectoId, string format);
}
