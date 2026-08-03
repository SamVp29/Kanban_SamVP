using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kanban.WebApi.Hubs
{
    [Authorize]
    public class KanbanHub : Hub
    {
        public async Task JoinBoardGroup(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
            await Clients.Caller.SendAsync("JoinedGroup", projectId);
        }

        public async Task LeaveBoardGroup(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);
            await Clients.Caller.SendAsync("LeftGroup", projectId);
        }
    }
}
