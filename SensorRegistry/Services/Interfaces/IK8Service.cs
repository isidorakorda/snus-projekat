using SensorRegistry.DTOs;
using SensorRegistry.Models;

namespace SensorRegistry.Services.Interfaces
{
    public interface IK8Service
    {
        Task ShutDownAndStartPod(Guid id);

    }
}
