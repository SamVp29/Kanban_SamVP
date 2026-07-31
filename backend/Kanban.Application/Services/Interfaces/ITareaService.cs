using Kanban.Application.DTOs;

namespace Kanban.Application.Services.Interfaces;

public interface ITareaService
{
    Task<IEnumerable<TareaResponseDto>> GetByColumnaIdAsync(int columnaId);
    Task<TareaResponseDto> CreateAsync(TareaCreateDto dto);
    Task<bool> UpdateAsync(int id, TareaCreateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> MoverTareaAsync(TareaMoveDto dto);
}
