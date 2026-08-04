namespace Kanban.Domain.Models;

public class ProyectoReportData
{
    public int ProyectoId { get; set; }
    public string NombreProyecto { get; set; } = string.Empty;
    public string DescripcionProyecto { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public List<ColumnaReportData> Columnas { get; set; } = new();
}

public class ColumnaReportData
{
    public string NombreColumna { get; set; } = string.Empty;
    public List<TareaReportData> Tareas { get; set; } = new();
}

public class TareaReportData
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public DateTime FechaCreacion { get; set; }
}
