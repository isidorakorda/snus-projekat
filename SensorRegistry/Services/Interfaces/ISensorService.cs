using SensorRegistry.DTOs;
using SensorRegistry.Models;

namespace SensorRegistry.Services.Interfaces
{
    public interface ISensorService
    {
        bool AddSensor(SensorRegistrationDTO data);
        Task<bool> SetSensorStatus(Guid id, bool isActive);
        Task ShutDownAndStartPod(Guid id);
        Task DeactivateFaultySensors();
        List<SensorDTO> GetAll(bool? isActive);

    }
}
