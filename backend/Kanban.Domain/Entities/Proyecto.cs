namespace Kanban.Domain.Entities;

public class Proyecto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFinPrevista { get; set; }
    public string Estado { get; set; } = "Activo"; // Ej. Activo, Finalizado

    public ICollection<Columna> Columnas { get; set; } = new List<Columna>();
}
