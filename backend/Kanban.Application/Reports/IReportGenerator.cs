using Kanban.Application.DTOs;

namespace Kanban.Application.Reports;

public interface IReportGenerator
{
    string Format { get; } // "pdf" o "excel"
    byte[] Generate(ProyectoReportDto data);
}
