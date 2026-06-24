using Server.DTO;

namespace Server.Service.IService
{
    public interface IAlarmPublisher
    {
        Task PublishAlarmAsync(AlarmDTO dto);
    }
}
