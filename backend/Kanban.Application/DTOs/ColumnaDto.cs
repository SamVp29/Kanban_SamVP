namespace Kanban.Application.DTOs;

public class ColumnaCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public int ProyectoId { get; set; }
}

public class ColumnaResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double Orden { get; set; }
    public int ProyectoId { get; set; }
}

public class ColumnaMoverDto
{
    public int ColumnaId { get; set; }
    public double NuevoOrden { get; set; }
}
