using FluentAssertions;
using Kanban.Application.Services;
using Kanban.Domain.Entities;
using Kanban.Domain.Ports.Out;
using Moq;
using Xunit;

namespace Kanban.UnitTests.Services;

public class ColumnaServiceTests
{
    private readonly Mock<IColumnaRepository> _columnaRepositoryMock;
    private readonly Mock<ITareaRepository> _tareaRepositoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly ColumnaService _columnaService;

    public ColumnaServiceTests()
    {
        _columnaRepositoryMock = new Mock<IColumnaRepository>();
        _tareaRepositoryMock = new Mock<ITareaRepository>();
        _boardNotifierMock = new Mock<IBoardNotifier>();
        _columnaService = new ColumnaService(_columnaRepositoryMock.Object, _tareaRepositoryMock.Object, _boardNotifierMock.Object);
    }

    [Fact]
    public async Task DeleteAsync_DebeLanzarExcepcion_CuandoColumnaTieneTareas()
    {
        int columnaId = 1;
        var columna = new Columna { Id = columnaId, Nombre = "En Progreso" };
        var tareasEnColumna = new List<Tarea>
        {
            new Tarea { Id = 101, Titulo = "Tarea pendiente", ColumnaId = columnaId }
        };

        _columnaRepositoryMock.Setup(r => r.GetByIdAsync(columnaId))
            .ReturnsAsync(columna);

        _tareaRepositoryMock.Setup(r => r.GetByColumnaIdAsync(columnaId))
            .ReturnsAsync(tareasEnColumna);

        Func<Task> act = async () => await _columnaService.DeleteAsync(columnaId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No se puede eliminar una columna que contiene tareas.");

        _columnaRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Columna>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_DebeEliminar_CuandoColumnaEstaVacia()
    {
        int columnaId = 2;
        var columnaVacia = new Columna { Id = columnaId, Nombre = "Completadas" };

        _columnaRepositoryMock.Setup(r => r.GetByIdAsync(columnaId))
            .ReturnsAsync(columnaVacia);

        _tareaRepositoryMock.Setup(r => r.GetByColumnaIdAsync(columnaId))
            .ReturnsAsync(new List<Tarea>());

        _columnaRepositoryMock.Setup(r => r.DeleteAsync(columnaVacia))
            .Returns(Task.CompletedTask);

        var result = await _columnaService.DeleteAsync(columnaId);

        result.Should().BeTrue();
        _columnaRepositoryMock.Verify(r => r.DeleteAsync(columnaVacia), Times.Once);
    }
}
