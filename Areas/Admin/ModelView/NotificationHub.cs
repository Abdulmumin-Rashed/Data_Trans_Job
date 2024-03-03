using Microsoft.AspNetCore.SignalR;

namespace Data_Trans_Job.Areas.Admin.ModelView
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            // Broadcast the notification message to all connected clients
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
