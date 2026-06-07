using Microsoft.AspNetCore.Mvc;
using SensorRegistry.DTOs;
using SensorRegistry.Services.Interfaces;

namespace SensorRegistry.Controllers
{
    [Route("api/sensor")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _service;
        private ILogger<SensorController> _logger;

        public SensorController(ISensorService service, ILogger<SensorController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult RegisterSensor([FromBody] SensorRegistrationDTO data)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool added = _service.AddSensor(data);

                if (!added) throw new Exception();

                _logger.LogInformation($"[SensorRegistry] Sensor Registered: {data.Id}");

                return Ok("[SensorRegistry] Sensor Registered");
            }catch(InvalidOperationException ex)
            {
                return StatusCode(409, ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "[SensorRegistry] Couldnt register new sensor");

                return StatusCode(500, "[SensorRegistry] An error occurred while registering the sensor");
            }
            
        }

        [HttpGet]
        public IActionResult GetAllSensors([FromQuery] bool? isActive)
        {
            List<SensorDTO> sensors = _service.GetAll(isActive);

            return Ok(sensors);
        }
    }
}
