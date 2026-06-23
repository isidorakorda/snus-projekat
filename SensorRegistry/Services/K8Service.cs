using k8s;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using SensorRegistry.BgServices;
using SensorRegistry.Data;
using SensorRegistry.DTOs;
using SensorRegistry.Models;
using SensorRegistry.Services.Interfaces;

namespace SensorRegistry.Services
{
    public class K8Service : IK8Service
    {
        private readonly IKubernetes _k8s;
        private readonly ILogger<K8Service> _logger;

        public K8Service(IKubernetes? k8s, ILogger<K8Service> logger)
        {
            _k8s = k8s;
            _logger = logger;
        }


        public async Task ShutDownAndStartPod(string podName)
        {
            if(_k8s == null)
            {
                _logger.LogWarning($"[SensorRegistry] K8s client not available. Skipping pod shutdown for {podName}");
                return;
            }

            _logger.LogInformation($"[SensorRegistry] Shutting down pod: sendorId={podName}");
            await _k8s.CoreV1.DeleteNamespacedPodAsync(podName, "sensors"); 
        }
    }
}
