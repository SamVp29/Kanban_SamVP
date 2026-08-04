using FluentAssertions;
using Kanban.Application.DTOs;
using Kanban.Application.Reports;
using Kanban.Domain.Entities;
using Kanban.Domain.Interfaces;
using Moq;
using Xunit;

namespace Kanban.UnitTests.Reports;

public class ReportServiceTests
{
    private readonly Mock<IReportGenerator> _generatorMock;
    private readonly Mock<IProyectoRepository> _proyectoRepositoryMock;
    private readonly ReportService _reportService;

    public ReportServiceTests()
    {
        _generatorMock = new Mock<IReportGenerator>();
        _generatorMock.Setup(g => g.Format).Returns("pdf");
        _generatorMock.Setup(g => g.Generate(It.IsAny<ProyectoReportDto>()))
            .Returns(new byte[] { 0x25, 0x50, 0x44, 0x46 }); // PDF magic bytes

        _proyectoRepositoryMock = new Mock<IProyectoRepository>();

        _reportService = new ReportService(
            new List<IReportGenerator> { _generatorMock.Object },
            _proyectoRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GenerateReportAsync_DebeGenerarReporte_CuandoProyectoExiste()
    {
        // Arrange
        int proyectoId = 1;
        var proyectoCompleto = new Proyecto
        {
            Id = proyectoId,
            Nombre = "Proyecto Test Reporte",
            Descripcion = "Descripción Test",
            FechaInicio = DateTime.UtcNow,
            Columnas = new List<Columna>
            {
                new Columna
                {
                    Id = 10,
                    Nombre = "Por Hacer",
                    Orden = 1,
                    Tareas = new List<Tarea>
                    {
                        new Tarea
                        {
                            Id = 100,
                            Titulo = "Tarea PDF",
                            Prioridad = "Alta",
                            Responsable = new Usuario { Id = 1, Nombre = "Admin" }
                        }
                    }
                }
            }
        };

        _proyectoRepositoryMock.Setup(r => r.GetProyectoCompletoReporteAsync(proyectoId))
            .ReturnsAsync(proyectoCompleto);

        // Act
        var result = await _reportService.GenerateReportAsync(proyectoId, "pdf");

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
        _generatorMock.Verify(g => g.Generate(It.Is<ProyectoReportDto>(dto => 
            dto.ProyectoId == proyectoId && 
            dto.Columnas.First().Tareas.First().Titulo == "Tarea PDF"
        )), Times.Once);
    }
}
