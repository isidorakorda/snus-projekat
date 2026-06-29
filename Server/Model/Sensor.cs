using System.ComponentModel.DataAnnotations;
using Server.Enum;

namespace Server.Model
{
    public class Sensor
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DataQuality Quality { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime DateTimeOfRegistration { get; set; } = DateTime.UtcNow;

        public DateTime? LastSeen { get; set; }

        public DateTime? DeactivationTime { get; set; }

        [Required]
        public string PublicKey { get; set; }

        [Required]
        public string PodName { get; set; }
    }
}
