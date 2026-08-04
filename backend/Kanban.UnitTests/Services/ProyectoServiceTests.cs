using FluentAssertions;
using Kanban.Application.Services;
using Kanban.Domain.Entities;
using Kanban.Domain.Interfaces;
using Moq;
using Xunit;

namespace Kanban.UnitTests.Services;

public class ProyectoServiceTests
{
    private readonly Mock<IProyectoRepository> _proyectoRepositoryMock;
    private readonly ProyectoService _proyectoService;

    public ProyectoServiceTests()
    {
        _proyectoRepositoryMock = new Mock<IProyectoRepository>();
        _proyectoService = new ProyectoService(_proyectoRepositoryMock.Object);
    }

    [Fact]
    public async Task GetPagedAsync_DebeRetornarPaginaYTotalCount()
    {
        // Arrange
        int page = 1;
        int pageSize = 5;
        string filtro = "Agile";

        var listaProyectos = new List<Proyecto>
        {
            new Proyecto { Id = 1, Nombre = "Proyecto Agile Scrum", Descripcion = "Desc 1" },
            new Proyecto { Id = 2, Nombre = "Proyecto Kanban Agile", Descripcion = "Desc 2" }
        };

        _proyectoRepositoryMock.Setup(r => r.GetPagedAsync(page, pageSize, filtro))
            .ReturnsAsync((listaProyectos, 2));

        // Act
        var result = await _proyectoService.GetPagedAsync(page, pageSize, filtro);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().Nombre.Should().Contain("Agile");
    }
}
