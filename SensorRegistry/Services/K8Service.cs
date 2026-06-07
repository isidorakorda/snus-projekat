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


        public async Task ShutDownAndStartPod(Guid id)
        {
            if(_k8s == null)
            {
                _logger.LogWarning($"[SensorRegistry] K8s client not available. Skipping pod shutdown for {id}");
                return;
            }

            V1PodList pods = await _k8s.CoreV1.ListNamespacedPodAsync("sensors", labelSelector: $"sensorId={id}");
            var podName = pods.Items.FirstOrDefault()?.Metadata.Name;

            if (podName != null){
                _logger.LogInformation($"[SensorRegistry] Shutting down pod: sendorId={id}");
                await _k8s.CoreV1.DeleteNamespacedPodAsync(podName, "sensors"); 
            }
        }
    }
}
