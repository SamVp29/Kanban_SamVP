using Kanban.Application.DTOs;
using Kanban.Application.Ports.In;
using Kanban.Domain.Entities;
using Kanban.Domain.Ports.Out;

namespace Kanban.Application.Services;

public class ColumnaService : IColumnaUseCase
{
    private readonly IColumnaRepository _columnaRepository;
    private readonly ITareaRepository _tareaRepository;
    private readonly IBoardNotifier _boardNotifier;

    public ColumnaService(
        IColumnaRepository columnaRepository,
        ITareaRepository tareaRepository,
        IBoardNotifier boardNotifier)
    {
        _columnaRepository = columnaRepository;
        _tareaRepository = tareaRepository;
        _boardNotifier = boardNotifier;
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
        await _boardNotifier.NotifyBoardUpdatedAsync(dto.ProyectoId);

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

        // Comportamiento de dominio rico (Rich Domain Model)
        columna.CambiarNombre(nuevoNombre);

        await _columnaRepository.UpdateAsync(columna);
        await _boardNotifier.NotifyBoardUpdatedAsync(columna.ProyectoId);
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

        var proyectoId = columna.ProyectoId;
        await _columnaRepository.DeleteAsync(columna);
        await _boardNotifier.NotifyBoardUpdatedAsync(proyectoId);
        return true;
    }

    public async Task<bool> ReordenarColumnaAsync(int columnaId, double nuevoOrden)
    {
        var columna = await _columnaRepository.GetByIdAsync(columnaId);
        if (columna == null) return false;

        // Comportamiento de dominio rico (Rich Domain Model)
        columna.Reordenar(nuevoOrden);

        await _columnaRepository.UpdateAsync(columna);
        await _boardNotifier.NotifyBoardUpdatedAsync(columna.ProyectoId);
        return true;
    }

    public async Task<int> GetProyectoIdByColumnaIdAsync(int id)
    {
        var columna = await _columnaRepository.GetByIdAsync(id);
        if (columna == null) throw new KeyNotFoundException("Columna no encontrada");
        return columna.ProyectoId;
    }
}
