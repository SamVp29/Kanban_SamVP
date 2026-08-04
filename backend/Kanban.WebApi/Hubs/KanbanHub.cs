using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kanban.WebApi.Hubs
{
    [Authorize]
    public class KanbanHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> BoardConnections = new();

        public async Task JoinBoardGroup(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
            
            lock (BoardConnections)
            {
                if (!BoardConnections.ContainsKey(projectId))
                {
                    BoardConnections[projectId] = new HashSet<string>();
                }
                BoardConnections[projectId].Add(Context.ConnectionId);
            }

            int count = GetConnectedCount(projectId);
            await Clients.Group(projectId).SendAsync("ConnectedUsersChanged", count);
        }

        public async Task LeaveBoardGroup(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);

            lock (BoardConnections)
            {
                if (BoardConnections.TryGetValue(projectId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        BoardConnections.TryRemove(projectId, out _);
                    }
                }
            }

            int count = GetConnectedCount(projectId);
            await Clients.Group(projectId).SendAsync("ConnectedUsersChanged", count);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            lock (BoardConnections)
            {
                foreach (var (projectId, connections) in BoardConnections)
                {
                    if (connections.Remove(Context.ConnectionId))
                    {
                        int count = connections.Count;
                        Clients.Group(projectId).SendAsync("ConnectedUsersChanged", count);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        private static int GetConnectedCount(string projectId)
        {
            lock (BoardConnections)
            {
                return BoardConnections.TryGetValue(projectId, out var connections) ? connections.Count : 0;
            }
        }
    }
}
