using Kanban.Domain.Models;

namespace Kanban.Domain.Ports.Out;

public interface IReportGenerator
{
    string Format { get; }
    byte[] Generate(ProyectoReportData data);
}
