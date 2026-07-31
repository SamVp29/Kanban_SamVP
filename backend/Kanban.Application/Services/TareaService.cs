using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Kanban.Domain.Entities;
using Kanban.Domain.Interfaces;

namespace Kanban.Application.Services;

public class TareaService : ITareaService
{
    private readonly ITareaRepository _tareaRepository;

    public TareaService(ITareaRepository tareaRepository)
    {
        _tareaRepository = tareaRepository;
    }

    public async Task<IEnumerable<TareaResponseDto>> GetByColumnaIdAsync(int columnaId)
    {
        var tareas = await _tareaRepository.GetByColumnaIdAsync(columnaId);
        return tareas.Select(t => new TareaResponseDto
        {
            Id = t.Id,
            Titulo = t.Titulo,
            Descripcion = t.Descripcion,
            Prioridad = t.Prioridad,
            Orden = t.Orden,
            FechaCreacion = t.FechaCreacion,
            ColumnaId = t.ColumnaId,
            ResponsableId = t.ResponsableId
        }).ToList();
    }

    public async Task<TareaResponseDto> CreateAsync(TareaCreateDto dto)
    {
        var tareasActuales = await _tareaRepository.GetByColumnaIdAsync(dto.ColumnaId);
        double maxOrden = tareasActuales.Any() ? tareasActuales.Max(t => t.Orden) : 0;

        var tarea = new Tarea
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Prioridad = dto.Prioridad,
            ColumnaId = dto.ColumnaId,
            ResponsableId = dto.ResponsableId,
            FechaCreacion = DateTime.UtcNow,
            Orden = maxOrden + 65536 // Lexical spacing
        };

        await _tareaRepository.AddAsync(tarea);

        return new TareaResponseDto
        {
            Id = tarea.Id,
            Titulo = tarea.Titulo,
            Descripcion = tarea.Descripcion,
            Prioridad = tarea.Prioridad,
            Orden = tarea.Orden,
            FechaCreacion = tarea.FechaCreacion,
            ColumnaId = tarea.ColumnaId,
            ResponsableId = tarea.ResponsableId
        };
    }

    public async Task<bool> UpdateAsync(int id, TareaCreateDto dto)
    {
        var tarea = await _tareaRepository.GetByIdAsync(id);
        if (tarea == null) return false;

        tarea.Titulo = dto.Titulo;
        tarea.Descripcion = dto.Descripcion;
        tarea.Prioridad = dto.Prioridad;
        tarea.ResponsableId = dto.ResponsableId;
        // ColumnaId y Orden se manejan con MoverTareaAsync

        await _tareaRepository.UpdateAsync(tarea);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tarea = await _tareaRepository.GetByIdAsync(id);
        if (tarea == null) return false;

        await _tareaRepository.DeleteAsync(tarea);
        return true;
    }

    public async Task<bool> MoverTareaAsync(TareaMoveDto dto)
    {
        var tarea = await _tareaRepository.GetByIdAsync(dto.TareaId);
        if (tarea == null) return false;

        tarea.ColumnaId = dto.NuevaColumnaId;
        tarea.Orden = dto.NuevoOrden;

        await _tareaRepository.UpdateAsync(tarea);
        return true;
    }
}
