using Microsoft.AspNetCore.SignalR;

namespace NutvaCms.API.Hubs
{
    public class ChatHub : Hub
    {
        // Called when a client joins the chat (website user or admin)
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }

        // Called when a client sends a message (can be used to broadcast in real-time)
        public async Task SendMessage(string sessionId, string sender, string text)
        {
            await Clients.Group(sessionId).SendAsync("ReceiveMessage", sender, text, DateTime.UtcNow);
        }
    }
}
