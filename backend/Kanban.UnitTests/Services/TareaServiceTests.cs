using FluentAssertions;
using Kanban.Application.DTOs;
using Kanban.Application.Services;
using Kanban.Domain.Entities;
using Kanban.Domain.Ports.Out;
using Moq;
using Xunit;

namespace Kanban.UnitTests.Services;

public class TareaServiceTests
{
    private readonly Mock<ITareaRepository> _tareaRepositoryMock;
    private readonly Mock<IColumnaRepository> _columnaRepositoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly TareaService _tareaService;

    public TareaServiceTests()
    {
        _tareaRepositoryMock = new Mock<ITareaRepository>();
        _columnaRepositoryMock = new Mock<IColumnaRepository>();
        _boardNotifierMock = new Mock<IBoardNotifier>();
        _tareaService = new TareaService(_tareaRepositoryMock.Object, _columnaRepositoryMock.Object, _boardNotifierMock.Object);
    }

    [Fact]
    public async Task CreateAsync_DebeCalcularNuevaPosicionLexicografica_CuandoSeAgregaUnaTarea()
    {
        int columnaId = 1;
        var tareasExistentes = new List<Tarea>
        {
            new Tarea { Id = 10, ColumnaId = columnaId, Orden = 65536 },
            new Tarea { Id = 11, ColumnaId = columnaId, Orden = 131072 }
        };

        _tareaRepositoryMock.Setup(r => r.GetByColumnaIdAsync(columnaId))
            .ReturnsAsync(tareasExistentes);

        Tarea? tareaInsertada = null;
        _tareaRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Tarea>()))
            .Callback<Tarea>(t => tareaInsertada = t)
            .Returns(Task.CompletedTask);

        var dto = new TareaCreateDto
        {
            Titulo = "Nueva Tarea de Prueba",
            Descripcion = "Descripción",
            Prioridad = "Alta",
            ColumnaId = columnaId
        };

        var result = await _tareaService.CreateAsync(dto);

        result.Should().NotBeNull();
        tareaInsertada.Should().NotBeNull();
        
        double ordenEsperado = 131072 + 65536;
        tareaInsertada!.Orden.Should().Be(ordenEsperado);
        result.Orden.Should().Be(ordenEsperado);
    }

    [Fact]
    public async Task UpdateColumnAsync_DebeActualizarColumnaYOrden_Correctamente()
    {
        int tareaId = 5;
        int nuevaColumnaId = 2;
        double nuevoOrden = 98304.5;

        var tareaExistente = new Tarea
        {
            Id = tareaId,
            Titulo = "Tarea a Mover",
            ColumnaId = 1,
            Orden = 65536
        };

        _tareaRepositoryMock.Setup(r => r.GetByIdAsync(tareaId))
            .ReturnsAsync(tareaExistente);

        _tareaRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Tarea>()))
            .Returns(Task.CompletedTask);

        var dto = new TareaMoveDto
        {
            TareaId = tareaId,
            NuevaColumnaId = nuevaColumnaId,
            NuevoOrden = nuevoOrden
        };

        var result = await _tareaService.MoverTareaAsync(dto);

        result.Should().BeTrue();
        tareaExistente.ColumnaId.Should().Be(nuevaColumnaId);
        tareaExistente.Orden.Should().Be(nuevoOrden);
        _tareaRepositoryMock.Verify(r => r.UpdateAsync(tareaExistente), Times.Once);
    }
}
