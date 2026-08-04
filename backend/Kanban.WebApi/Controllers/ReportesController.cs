using Kanban.Application.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportesController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("{proyectoId:int}")]
    public async Task<IActionResult> Export(
        int proyectoId, 
        [FromQuery] string format = "pdf",
        [FromQuery] string? prioridad = null,
        [FromQuery] int? responsableId = null,
        [FromQuery] string? texto = null)
    {
        try
        {
            var content = await _reportService.GenerateReportAsync(proyectoId, format, prioridad, responsableId, texto);
            
            if (format.ToLower() == "pdf")
            {
                return File(content, "application/pdf", $"Tablero_{proyectoId}.pdf");
            }
            else if (format.ToLower() == "excel")
            {
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Tablero_{proyectoId}.xlsx");
            }
            
            return BadRequest("Formato no soportado");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Proyecto no encontrado");
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
