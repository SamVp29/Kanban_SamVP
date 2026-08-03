namespace Kanban.Application.DTOs;

public class ProyectoReportDto
{
    public int ProyectoId { get; set; }
    public string NombreProyecto { get; set; } = string.Empty;
    public string DescripcionProyecto { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public List<ColumnaReportDto> Columnas { get; set; } = new();
}

public class ColumnaReportDto
{
    public string NombreColumna { get; set; } = string.Empty;
    public List<TareaReportDto> Tareas { get; set; } = new();
}

public class TareaReportDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public DateTime FechaCreacion { get; set; }
}
