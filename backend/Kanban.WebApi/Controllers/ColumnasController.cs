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
public class ColumnasController : ControllerBase
{
    private readonly IColumnaService _columnaService;
    private readonly IHubContext<KanbanHub> _hubContext;

    public ColumnasController(IColumnaService columnaService, IHubContext<KanbanHub> hubContext)
    {
        _columnaService = columnaService;
        _hubContext = hubContext;
    }

    [HttpGet("proyecto/{proyectoId:int}")]
    public async Task<ActionResult<IEnumerable<ColumnaResponseDto>>> GetByProyecto(int proyectoId)
    {
        var columnas = await _columnaService.GetByProyectoIdAsync(proyectoId);
        return Ok(columnas);
    }

    [HttpPost]
    public async Task<ActionResult<ColumnaResponseDto>> Create(ColumnaCreateDto dto)
    {
        var columna = await _columnaService.CreateAsync(dto);
        await _hubContext.Clients.Group(dto.ProyectoId.ToString()).SendAsync("BoardUpdated");
        return Created("", columna);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] string nuevoNombre)
    {
        var proyectoId = await _columnaService.GetProyectoIdByColumnaIdAsync(id);
        var success = await _columnaService.UpdateAsync(id, nuevoNombre);
        if (!success) return NotFound();
        await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
        return NoContent();
    }

    [HttpPut("mover")]
    public async Task<ActionResult> Mover([FromBody] ColumnaMoverDto dto)
    {
        var proyectoId = await _columnaService.GetProyectoIdByColumnaIdAsync(dto.ColumnaId);
        var success = await _columnaService.ReordenarColumnaAsync(dto.ColumnaId, dto.NuevoOrden);
        if (!success) return NotFound();
        await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var proyectoId = await _columnaService.GetProyectoIdByColumnaIdAsync(id);
            var success = await _columnaService.DeleteAsync(id);
            if (!success) return NotFound();
            await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
