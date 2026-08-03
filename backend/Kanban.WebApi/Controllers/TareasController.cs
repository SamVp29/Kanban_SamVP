using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Kanban.WebApi.Hubs;

namespace Kanban.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TareasController : ControllerBase
{
    private readonly ITareaService _tareaService;
    private readonly IColumnaService _columnaService;
    private readonly IHubContext<KanbanHub> _hubContext;

    public TareasController(ITareaService tareaService, IColumnaService columnaService, IHubContext<KanbanHub> hubContext)
    {
        _tareaService = tareaService;
        _columnaService = columnaService;
        _hubContext = hubContext;
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
        var proyectoId = await _columnaService.GetProyectoIdByColumnaIdAsync(dto.ColumnaId);
        await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
        return Created("", tarea);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] TareaCreateDto dto)
    {
        var success = await _tareaService.UpdateAsync(id, dto);
        if (!success) return NotFound();
        var proyectoId = await _columnaService.GetProyectoIdByColumnaIdAsync(dto.ColumnaId);
        await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var columnaId = await _tareaService.GetColumnaIdByTareaIdAsync(id);
            var proyectoId = await _columnaService.GetProyectoIdByColumnaIdAsync(columnaId);
            
            var success = await _tareaService.DeleteAsync(id);
            if (!success) return NotFound();
            
            await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
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
        var success = await _tareaService.MoverTareaAsync(dto);
        if (!success) return NotFound();
        var proyectoId = await _columnaService.GetProyectoIdByColumnaIdAsync(dto.NuevaColumnaId);
        await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
        return NoContent();
    }
}
