namespace Server.Service.IService
{
    public interface IAlarmPublisher
    {
        Task PublishAlarmAsync(string message);
    }
}
