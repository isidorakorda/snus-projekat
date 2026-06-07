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
    public class SensorService : ISensorService
    {
        private readonly SensorDbContext _context;
        private readonly IKubernetes _k8s;
        private readonly ILogger<SensorService> _logger;

        public SensorService(SensorDbContext context, IKubernetes? k8s, ILogger<SensorService> logger)
        {
            _context = context;
            _k8s = k8s;
            _logger = logger;
        }

        public bool AddSensor(SensorRegistrationDTO data)
        {
            if(_context.Sensors.Find(data.Id) != null)
            {
                throw new InvalidOperationException($"[SensorRegistry] Sensor ID already exists");
            }

            Sensor sensor = new Sensor
            {
                Id = data.Id,
                Quality = data.Quality,
                PublicKey = data.PublicKey,
                IsActive = true,
                IsMalicious = false,
                DateTimeOfRegistration = DateTime.UtcNow
            };

            try
            {
                _context.Sensors.Add(sensor);
                _logger.LogInformation($"[SensorRegistry] Added new sensor to registry");
            }catch(Exception ex)
            {
                _logger.LogError(ex, "[SensorRegistry] Error occurred while adding new sensors");
                return false;
            }
            

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> SetSensorStatus(Guid id, bool isActive)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null) {
                _logger.LogError($"[SensorRegistry] Couldnt change status for sensorId={id}");
                return false; 
            }

            if(isActive == false) sensor.DeactivationTime = DateTime.UtcNow; 
            sensor.IsActive = isActive;

            _logger.LogInformation($"[SensorRegistry] Changed sensor status: sendorId={id}");

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task ShutDownAndStartPod(Guid id)
        {
            if(_k8s == null)
            {
                _logger.LogWarning($"[SensorRegistry] K8s client not available. Skipping pod shutdown for {id}");
                return;
            }

            var pods = await _k8s.CoreV1.ListNamespacedPodAsync("sensors", labelSelector: $"sensorId={id}");
            var podName = pods.Items.FirstOrDefault()?.Metadata.Name;

            if (podName != null){
                _logger.LogInformation($"[SensorRegistry] Shutting down pod: sendorId={id}");
                await _k8s.CoreV1.DeleteNamespacedPodAsync(podName, "sensors"); 
            }
        }

        public async Task DeactivateFaultySensors()
        {
            var timeout = DateTime.UtcNow.AddSeconds(-10);
            // treba promeniti bazu iz koje cita podatke, kako bi gledao poruke a ne vreme aktivacije
            List<Sensor> expiredSensors = await _context.Sensors.Where(s => s.IsActive && s.DateTimeOfRegistration < timeout).ToListAsync();
            _logger.LogInformation($"[SensorRegistry] Found {expiredSensors.Count.ToString()} faulty sensors");

            foreach(Sensor sensor in expiredSensors)
            {
                await this.SetSensorStatus(sensor.Id, false);
                await this.ShutDownAndStartPod(sensor.Id);
            }

            await _context.SaveChangesAsync();
        }

        public List<SensorDTO> GetAll(bool? isActive)
        {
            IQueryable<Sensor> query = _context.Sensors.AsQueryable();

            if (isActive.HasValue) query = query.Where(s => s.IsActive == isActive.Value);

            List<Sensor> sensors = query.ToList();

            return query.Select(model => new SensorDTO
            {
                Id = model.Id,
                Quality = model.Quality,
                IsActive = model.IsActive,
                IsMalicious = model.IsMalicious,
                DateTimeOfRegistration = model.DateTimeOfRegistration,
                LastSeen = model.LastSeen,
                DeactivationTime = model.DeactivationTime,
                PublicKey = model.PublicKey
            }).ToList();

        }
    }
}
