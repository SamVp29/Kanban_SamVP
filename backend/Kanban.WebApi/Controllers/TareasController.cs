using Kanban.Application.DTOs;
using Kanban.Application.Ports.In;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TareasController : ControllerBase
{
    private readonly ITareaUseCase _tareaUseCase;

    public TareasController(ITareaUseCase tareaUseCase)
    {
        _tareaUseCase = tareaUseCase;
    }

    [HttpGet("columna/{columnaId:int}")]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetByColumna(int columnaId)
    {
        var tareas = await _tareaUseCase.GetByColumnaIdAsync(columnaId);
        return Ok(tareas);
    }

    [HttpPost]
    public async Task<ActionResult<TareaResponseDto>> Create(TareaCreateDto dto)
    {
        var tarea = await _tareaUseCase.CreateAsync(dto);
        return Created("", tarea);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] TareaCreateDto dto)
    {
        var success = await _tareaUseCase.UpdateAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var success = await _tareaUseCase.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("mover")]
    public async Task<ActionResult> Mover([FromBody] TareaMoveDto dto)
    {
        var success = await _tareaUseCase.MoverTareaAsync(dto);
        if (!success) return NotFound();
        return NoContent();
    }
}
