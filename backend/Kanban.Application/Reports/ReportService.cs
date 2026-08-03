using Kanban.Application.DTOs;
using Kanban.Domain.Interfaces;

namespace Kanban.Application.Reports;

public class ReportService : IReportService
{
    private readonly IEnumerable<IReportGenerator> _generators;
    private readonly IProyectoRepository _proyectoRepository;
    private readonly IColumnaRepository _columnaRepository;
    private readonly ITareaRepository _tareaRepository;

    public ReportService(
        IEnumerable<IReportGenerator> generators,
        IProyectoRepository proyectoRepository,
        IColumnaRepository columnaRepository,
        ITareaRepository tareaRepository)
    {
        _generators = generators;
        _proyectoRepository = proyectoRepository;
        _columnaRepository = columnaRepository;
        _tareaRepository = tareaRepository;
    }

    public async Task<byte[]> GenerateReportAsync(int proyectoId, string format)
    {
        var generator = _generators.FirstOrDefault(g => g.Format.Equals(format, StringComparison.OrdinalIgnoreCase));
        if (generator == null)
        {
            throw new NotSupportedException($"El formato de reporte '{format}' no está soportado.");
        }

        var data = await BuildReportDataAsync(proyectoId);
        return generator.Generate(data);
    }

    private async Task<ProyectoReportDto> BuildReportDataAsync(int proyectoId)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(proyectoId);
        if (proyecto == null) throw new KeyNotFoundException("Proyecto no encontrado");

        var columnas = await _columnaRepository.GetByProyectoIdAsync(proyectoId);
        var reportData = new ProyectoReportDto
        {
            ProyectoId = proyecto.Id,
            NombreProyecto = proyecto.Nombre,
            DescripcionProyecto = proyecto.Descripcion,
            FechaCreacion = proyecto.FechaInicio,
            Columnas = new List<ColumnaReportDto>()
        };

        foreach (var col in columnas.OrderBy(c => c.Orden))
        {
            var tareas = await _tareaRepository.GetByColumnaIdAsync(col.Id);
            var colDto = new ColumnaReportDto
            {
                NombreColumna = col.Nombre,
                Tareas = tareas.OrderBy(t => t.Orden).Select(t => new TareaReportDto
                {
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Prioridad = t.Prioridad,
                    FechaCreacion = t.FechaCreacion,
                    // Si tienes el Responsable incluido, podrías usar t.Responsable?.Nombre. Por ahora null.
                    Responsable = null 
                }).ToList()
            };
            reportData.Columnas.Add(colDto);
        }

        return reportData;
    }
}
