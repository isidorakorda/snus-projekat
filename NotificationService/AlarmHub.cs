using Microsoft.AspNetCore.SignalR;

namespace NotificationService
{
    public class AlarmHub : Hub
    {
        public async Task InitSub()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AlarmNotifications");
        }

        public async Task SendAlarm(string message)
        {
            Console.WriteLine($"[SERVER] sending alarm: {message}");
            await Clients.Group("AlarmNotifications").SendAsync("Alarm", message);
        }
    }
}
