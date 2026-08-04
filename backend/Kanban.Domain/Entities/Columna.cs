namespace Kanban.Domain.Entities;

public class Columna
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double Orden { get; set; } // Float/Double for easy reordering (Lexicographical logic)

    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    // Reglas y comportamientos del Dominio
    public void CambiarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nombre de la columna no puede estar vacío.");
        Nombre = nuevoNombre;
    }

    public void Reordenar(double nuevoOrden)
    {
        Orden = nuevoOrden;
    }
}
