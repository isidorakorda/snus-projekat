using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensorRegistry.DTOs;
using SensorRegistry.Features.Commands;
using SensorRegistry.Features.Queries;
using SensorRegistry.Services.Interfaces;
using System.Threading.Tasks;

namespace SensorRegistry.Controllers
{
    [Route("api/sensor")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly IK8Service _service;
        private readonly IMediator _mediator;
        private ILogger<SensorController> _logger;

        public SensorController(IK8Service service, ILogger<SensorController> logger, IMediator mediator)
        {
            _service = service;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterSensor([FromBody] SensorRegistrationDTO data, CancellationToken token)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool added = await _mediator.Send(new AddSensorCommand(data), token);

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
        public async Task<IActionResult> GetAllSensors([FromQuery] bool? isActive, CancellationToken token)
        {
            List<SensorDTO> sensors = await _mediator.Send(new GetAllSensorsQuery(isActive), token);

            return Ok(sensors);
        }
    }
}
