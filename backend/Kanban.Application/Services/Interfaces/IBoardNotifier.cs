namespace Kanban.Application.Services.Interfaces;

public interface IBoardNotifier
{
    Task NotifyBoardUpdatedAsync(int proyectoId);
}
