namespace Kanban.Domain.Ports.Out;

public interface IBoardNotifier
{
    Task NotifyBoardUpdatedAsync(int proyectoId);
}
