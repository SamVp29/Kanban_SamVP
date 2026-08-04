using Kanban.Domain.Ports.Out;
using Kanban.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kanban.WebApi.Adapters;

public class SignalRBoardNotifier : IBoardNotifier
{
    private readonly IHubContext<KanbanHub> _hubContext;

    public SignalRBoardNotifier(IHubContext<KanbanHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyBoardUpdatedAsync(int proyectoId)
    {
        await _hubContext.Clients.Group(proyectoId.ToString()).SendAsync("BoardUpdated");
    }
}
