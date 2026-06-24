using Microsoft.AspNetCore.SignalR.Client;
using Server.DTO;
using Server.Service.IService;

namespace Server.Service
{
    public class AlarmPublisher : IAlarmPublisher, IAsyncDisposable
    {
        private readonly HubConnection _connection;

        public AlarmPublisher(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }

        public async Task PublishAlarmAsync(AlarmDTO dto)
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }

            await _connection.InvokeAsync("SendAlarm", dto);
        }
    }
}
