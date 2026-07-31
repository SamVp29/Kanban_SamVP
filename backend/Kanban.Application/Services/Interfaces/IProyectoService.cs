using Kanban.Application.DTOs;

namespace Kanban.Application.Services.Interfaces;

public interface IProyectoService
{
    Task<PagedResponseDto<ProyectoResponseDto>> GetPagedAsync(int page, int pageSize, string? nombreFiltro);
    Task<IEnumerable<ProyectoResponseDto>> GetAllAsync();
    Task<ProyectoResponseDto?> GetByIdAsync(int id);
    Task<ProyectoResponseDto> CreateAsync(ProyectoCreateDto dto);
    Task<bool> UpdateAsync(int id, ProyectoCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
