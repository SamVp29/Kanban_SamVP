namespace Kanban.Domain.Entities;

public class Columna
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double Orden { get; set; } // Float/Double for easy reordering (Lexicographical logic)

    public Guid ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
