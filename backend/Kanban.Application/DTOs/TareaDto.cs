namespace Kanban.Application.DTOs;

public class TareaCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty; // Alta, Media, Baja
    public int ColumnaId { get; set; }
    public int? ResponsableId { get; set; }
}

public class TareaResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public double Orden { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int ColumnaId { get; set; }
    public int? ResponsableId { get; set; }
}

public class TareaMoveDto
{
    public int TareaId { get; set; }
    public int NuevaColumnaId { get; set; }
    public double NuevoOrden { get; set; }
}
