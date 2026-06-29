using Microsoft.AspNetCore.SignalR;
using NotificationService.DTO;

namespace NotificationService
{
    public class AlarmHub : Hub
    {
        public async Task InitSub()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AlarmNotifications");
        }

        public async Task SendAlarm(AlarmDTO dto)
        {
            Console.WriteLine($"[SERVER] sending alarm: {dto.Message}");
            await Clients.Group("AlarmNotifications").SendAsync("Alarm", dto);
        }
    }
}
