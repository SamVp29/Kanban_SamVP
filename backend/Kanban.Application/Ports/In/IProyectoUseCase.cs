using Kanban.Application.DTOs;

namespace Kanban.Application.Ports.In;

public interface IProyectoUseCase
{
    Task<IEnumerable<ProyectoResponseDto>> GetAllAsync();
    Task<PagedResponseDto<ProyectoResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search);
    Task<ProyectoResponseDto?> GetByIdAsync(int id);
    Task<ProyectoResponseDto> CreateAsync(ProyectoCreateDto dto);
    Task<bool> UpdateAsync(int id, ProyectoCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
