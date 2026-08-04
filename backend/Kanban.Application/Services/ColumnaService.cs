using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Kanban.Domain.Entities;
using Kanban.Domain.Interfaces;

namespace Kanban.Application.Services;

public class ColumnaService : IColumnaService
{
    private readonly IColumnaRepository _columnaRepository;
    private readonly ITareaRepository _tareaRepository;

    public ColumnaService(IColumnaRepository columnaRepository, ITareaRepository tareaRepository)
    {
        _columnaRepository = columnaRepository;
        _tareaRepository = tareaRepository;
    }

    public async Task<IEnumerable<ColumnaResponseDto>> GetByProyectoIdAsync(int proyectoId)
    {
        var columnas = await _columnaRepository.GetByProyectoIdAsync(proyectoId);
        return columnas.Select(c => new ColumnaResponseDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Orden = c.Orden,
            ProyectoId = c.ProyectoId
        }).ToList();
    }

    public async Task<ColumnaResponseDto> CreateAsync(ColumnaCreateDto dto)
    {
        var columnasActuales = await _columnaRepository.GetByProyectoIdAsync(dto.ProyectoId);
        double maxOrden = columnasActuales.Any() ? columnasActuales.Max(c => c.Orden) : 0;

        var columna = new Columna
        {
            Nombre = dto.Nombre,
            ProyectoId = dto.ProyectoId,
            Orden = maxOrden + 65536 // Usamos espaciado léxico simple
        };

        await _columnaRepository.AddAsync(columna);

        return new ColumnaResponseDto
        {
            Id = columna.Id,
            Nombre = columna.Nombre,
            Orden = columna.Orden,
            ProyectoId = columna.ProyectoId
        };
    }

    public async Task<bool> UpdateAsync(int id, string nuevoNombre)
    {
        var columna = await _columnaRepository.GetByIdAsync(id);
        if (columna == null) return false;

        columna.Nombre = nuevoNombre;
        await _columnaRepository.UpdateAsync(columna);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var columna = await _columnaRepository.GetByIdAsync(id);
        if (columna == null) return false;

        // Regla de Negocio: No se puede eliminar una columna si tiene tareas
        var tareas = await _tareaRepository.GetByColumnaIdAsync(id);
        if (tareas.Any())
        {
            throw new InvalidOperationException("No se puede eliminar una columna que contiene tareas.");
        }

        await _columnaRepository.DeleteAsync(columna);
        return true;
    }

    public async Task<bool> ReordenarColumnaAsync(int columnaId, double nuevoOrden)
    {
        var columna = await _columnaRepository.GetByIdAsync(columnaId);
        if (columna == null) return false;

        columna.Orden = nuevoOrden;
        await _columnaRepository.UpdateAsync(columna);
        return true;
    }

    public async Task<int> GetProyectoIdByColumnaIdAsync(int id)
    {
        var columna = await _columnaRepository.GetByIdAsync(id);
        if (columna == null) throw new KeyNotFoundException("Columna no encontrada");
        return columna.ProyectoId;
    }
}
