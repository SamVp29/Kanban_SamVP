using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProyectosController : ControllerBase
{
    private readonly IProyectoService _proyectoService;

    public ProyectosController(IProyectoService proyectoService)
    {
        _proyectoService = proyectoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<ProyectoResponseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filtro = null)
    {
        var proyectos = await _proyectoService.GetPagedAsync(page, pageSize, filtro);
        return Ok(proyectos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var proyecto = await _proyectoService.GetByIdAsync(id);
        if (proyecto == null) return NotFound();
        return Ok(proyecto);
    }

    [HttpPost]
    public async Task<ActionResult<ProyectoResponseDto>> Create(ProyectoCreateDto dto)
    {
        var proyecto = await _proyectoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = proyecto.Id }, proyecto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProyectoCreateDto dto)
    {
        var success = await _proyectoService.UpdateAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _proyectoService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
