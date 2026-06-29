using ConsensusService.Enum;

namespace ConsensusService.Model
{
    public class Sensor
    {
        public Guid Id { get; set; }
        public DataQuality Quality { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateTimeOfRegistration { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeen { get; set; }
        public DateTime? DeactivationTime { get; set; }
        public string PublicKey { get; set; }
        public string PodName { get; set; }
    }
}
