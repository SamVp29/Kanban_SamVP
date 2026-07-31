namespace Kanban.Domain.Entities;

public class Tarea
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = "Media"; // Alta, Media, Baja
    
    public Guid? ResponsableId { get; set; }
    public Usuario? Responsable { get; set; }

    public Guid ColumnaId { get; set; }
    public Columna Columna { get; set; } = null!;

    public double Orden { get; set; } // Lexicographical ordering / rank
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
