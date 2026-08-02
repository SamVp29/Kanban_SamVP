using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TareasController : ControllerBase
{
    private readonly ITareaService _tareaService;

    public TareasController(ITareaService tareaService)
    {
        _tareaService = tareaService;
    }

    [HttpGet("columna/{columnaId:int}")]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetByColumna(int columnaId)
    {
        var tareas = await _tareaService.GetByColumnaIdAsync(columnaId);
        return Ok(tareas);
    }

    [HttpPost]
    public async Task<ActionResult<TareaResponseDto>> Create(TareaCreateDto dto)
    {
        var tarea = await _tareaService.CreateAsync(dto);
        return Created("", tarea);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] TareaCreateDto dto)
    {
        var success = await _tareaService.UpdateAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _tareaService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("mover")]
    public async Task<ActionResult> Mover([FromBody] TareaMoveDto dto)
    {
        var success = await _tareaService.MoverTareaAsync(dto);
        if (!success) return NotFound();
        return NoContent();
    }
}
