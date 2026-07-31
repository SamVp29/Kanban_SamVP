using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColumnasController : ControllerBase
{
    private readonly IColumnaService _columnaService;

    public ColumnasController(IColumnaService columnaService)
    {
        _columnaService = columnaService;
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
        return Created("", columna); // Idealmente usar CreatedAtAction pero la ruta es diferente
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] string nuevoNombre)
    {
        var success = await _columnaService.UpdateAsync(id, nuevoNombre);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var success = await _columnaService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
