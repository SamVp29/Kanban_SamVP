using Kanban.Application.DTOs;

namespace Kanban.Application.Services.Interfaces;

public interface IColumnaService
{
    Task<IEnumerable<ColumnaResponseDto>> GetByProyectoIdAsync(int proyectoId);
    Task<ColumnaResponseDto> CreateAsync(ColumnaCreateDto dto);
    Task<bool> UpdateAsync(int id, string nuevoNombre);
    Task<bool> DeleteAsync(int id);
    Task<bool> ReordenarColumnaAsync(int columnaId, double nuevoOrden);
    Task<int> GetProyectoIdByColumnaIdAsync(int id);
}
