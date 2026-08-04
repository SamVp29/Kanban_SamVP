using Kanban.Application.Ports.In;
using Kanban.Domain.Models;
using Kanban.Domain.Ports.Out;

namespace Kanban.Application.Reports;

public class ReportService : IReportUseCase
{
    private readonly IEnumerable<IReportGenerator> _generators;
    private readonly IProyectoRepository _proyectoRepository;

    public ReportService(
        IEnumerable<IReportGenerator> generators,
        IProyectoRepository proyectoRepository)
    {
        _generators = generators;
        _proyectoRepository = proyectoRepository;
    }

    public async Task<byte[]> GenerateReportAsync(int proyectoId, string format, string? prioridad = null, int? responsableId = null, string? texto = null)
    {
        var generator = _generators.FirstOrDefault(g => g.Format.Equals(format, StringComparison.OrdinalIgnoreCase));
        if (generator == null)
        {
            throw new NotSupportedException($"El formato de reporte '{format}' no está soportado.");
        }

        var data = await BuildReportDataAsync(proyectoId, prioridad, responsableId, texto);
        return generator.Generate(data);
    }

    private async Task<ProyectoReportData> BuildReportDataAsync(int proyectoId, string? prioridad, int? responsableId, string? texto)
    {
        var proyecto = await _proyectoRepository.GetProyectoCompletoReporteAsync(proyectoId);
        if (proyecto == null) throw new KeyNotFoundException("Proyecto no encontrado");

        var reportData = new ProyectoReportData
        {
            ProyectoId = proyecto.Id,
            NombreProyecto = proyecto.Nombre,
            DescripcionProyecto = proyecto.Descripcion,
            FechaCreacion = proyecto.FechaInicio,
            Columnas = new List<ColumnaReportData>()
        };

        foreach (var col in proyecto.Columnas.OrderBy(c => c.Orden))
        {
            var tareasQuery = col.Tareas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(prioridad))
            {
                tareasQuery = tareasQuery.Where(t => t.Prioridad.Equals(prioridad, StringComparison.OrdinalIgnoreCase));
            }

            if (responsableId.HasValue)
            {
                tareasQuery = tareasQuery.Where(t => t.ResponsableId == responsableId.Value);
            }

            if (!string.IsNullOrWhiteSpace(texto))
            {
                tareasQuery = tareasQuery.Where(t => 
                    t.Titulo.ToLower().Contains(texto.ToLower()) || 
                    t.Descripcion.ToLower().Contains(texto.ToLower()));
            }

            var colDto = new ColumnaReportData
            {
                NombreColumna = col.Nombre,
                Tareas = tareasQuery.OrderBy(t => t.Orden).Select(t => new TareaReportData
                {
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Prioridad = t.Prioridad,
                    FechaCreacion = t.FechaCreacion,
                    Responsable = t.Responsable?.Nombre ?? "Sin asignar"
                }).ToList()
            };
            reportData.Columnas.Add(colDto);
        }

        return reportData;
    }
}
