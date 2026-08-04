using Kanban.Application.DTOs;
using Kanban.Application.Ports.In;
using Kanban.Domain.Entities;
using Kanban.Domain.Ports.Out;

namespace Kanban.Application.Services;

public class ProyectoService : IProyectoUseCase
{
    private readonly IProyectoRepository _proyectoRepository;

    public ProyectoService(IProyectoRepository proyectoRepository)
    {
        _proyectoRepository = proyectoRepository;
    }

    public async Task<PagedResponseDto<ProyectoResponseDto>> GetPagedAsync(int page, int pageSize, string? nombreFiltro)
    {
        var items = await _proyectoRepository.GetPagedAsync(page, pageSize, nombreFiltro);
        var totalCount = await _proyectoRepository.GetTotalCountAsync(nombreFiltro);

        var dtoList = items.Select(p => new ProyectoResponseDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            FechaInicio = p.FechaInicio,
            FechaFinPrevista = p.FechaFinPrevista,
            Estado = p.Estado
        }).ToList();

        return new PagedResponseDto<ProyectoResponseDto>
        {
            Items = dtoList,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<ProyectoResponseDto>> GetAllAsync()
    {
        var proyectos = await _proyectoRepository.GetAllAsync();
        return proyectos.Select(p => new ProyectoResponseDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            FechaInicio = p.FechaInicio,
            FechaFinPrevista = p.FechaFinPrevista,
            Estado = p.Estado
        }).ToList();
    }

    public async Task<ProyectoResponseDto?> GetByIdAsync(int id)
    {
        var p = await _proyectoRepository.GetByIdAsync(id);
        if (p == null) return null;

        return new ProyectoResponseDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            FechaInicio = p.FechaInicio,
            FechaFinPrevista = p.FechaFinPrevista,
            Estado = p.Estado
        };
    }

    public async Task<ProyectoResponseDto> CreateAsync(ProyectoCreateDto dto)
    {
        var proyecto = new Proyecto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            FechaInicio = dto.FechaInicio.ToUniversalTime(),
            FechaFinPrevista = dto.FechaFinPrevista?.ToUniversalTime(),
            Estado = "Activo"
        };

        await _proyectoRepository.AddAsync(proyecto);

        return new ProyectoResponseDto
        {
            Id = proyecto.Id,
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            FechaInicio = proyecto.FechaInicio,
            FechaFinPrevista = proyecto.FechaFinPrevista,
            Estado = proyecto.Estado
        };
    }

    public async Task<bool> UpdateAsync(int id, ProyectoCreateDto dto)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id);
        if (proyecto == null) return false;

        proyecto.Nombre = dto.Nombre;
        proyecto.Descripcion = dto.Descripcion;
        proyecto.FechaInicio = dto.FechaInicio.ToUniversalTime();
        proyecto.FechaFinPrevista = dto.FechaFinPrevista?.ToUniversalTime();

        await _proyectoRepository.UpdateAsync(proyecto);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id);
        if (proyecto == null) return false;

        await _proyectoRepository.DeleteAsync(proyecto);
        return true;
    }
}
