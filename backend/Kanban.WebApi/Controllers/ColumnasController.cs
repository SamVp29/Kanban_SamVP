using Kanban.Application.DTOs;
using Kanban.Application.Ports.In;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ColumnasController : ControllerBase
{
    private readonly IColumnaUseCase _columnaUseCase;

    public ColumnasController(IColumnaUseCase columnaUseCase)
    {
        _columnaUseCase = columnaUseCase;
    }

    [HttpGet("proyecto/{proyectoId:int}")]
    public async Task<ActionResult<IEnumerable<ColumnaResponseDto>>> GetByProyecto(int proyectoId)
    {
        var columnas = await _columnaUseCase.GetByProyectoIdAsync(proyectoId);
        return Ok(columnas);
    }

    [HttpPost]
    public async Task<ActionResult<ColumnaResponseDto>> Create(ColumnaCreateDto dto)
    {
        var columna = await _columnaUseCase.CreateAsync(dto);
        return Created("", columna);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] string nuevoNombre)
    {
        var success = await _columnaUseCase.UpdateAsync(id, nuevoNombre);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("mover")]
    public async Task<ActionResult> Mover([FromBody] ColumnaMoverDto dto)
    {
        var success = await _columnaUseCase.ReordenarColumnaAsync(dto.ColumnaId, dto.NuevoOrden);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var success = await _columnaUseCase.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
