using SensorRegistry.Enums;
using System.ComponentModel.DataAnnotations;

namespace SensorRegistry.DTOs
{
    public class SensorRegistrationDTO
    {
        [Required(ErrorMessage = "[SensorRegistry] Sensor must contain unique ID")]
        public Guid Id { get; set; }

        [EnumDataType(typeof(DataQuality), ErrorMessage = "[SensorRegistry] Quality of sensor must exist")]
        public DataQuality Quality { get; set; }

        [Required(ErrorMessage = "[SensorRegistry] Public key is required")]
        [StringLength(500, MinimumLength = 32, ErrorMessage = "[SensorRegistry] Public key length must be between 32 and 500 chars")]
        public string PublicKey { get; set; }

        [Required(ErrorMessage = "[SensorRegistry] Pod Name is required")]
        public string PodName { get; set; }
    }
}
