using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using Server.Features.Commands;
using System.Collections.Concurrent;

namespace Server.Controller
{
    [ApiController]
    [Route("api/ingestion")]
    public class IngestionController : ControllerBase
    {
        private readonly IMediator mediator;

        private static readonly ConcurrentDictionary<Guid, bool> blockedSensors = new();

        public IngestionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("ingest")]
        public async Task<IActionResult> Ingest([FromBody] IngestDataDTO dto)
        {
            if (dto == null) return BadRequest("Not valid");

            if (blockedSensors.ContainsKey(dto.SensorId))
                return Ok(new {message = "Sent message"} );

            var success = await mediator.Send(new IngestDataCommand(dto));

            if (success)
            {
                return Ok(new { message = "Data successfully processed and saved" });
            }

            return StatusCode(500, "Internal Server Error occurred while trying to save data");
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockSensor([FromBody] string sensorIdInput)
        {
            if (string.IsNullOrWhiteSpace(sensorIdInput))
                return BadRequest("Sensor Id cannot be null or empty");

            if (!Guid.TryParse(sensorIdInput.Trim(), out Guid sensorId))
            {
                return BadRequest($"Invalid Guid format: {sensorIdInput}");
            }

            bool isNewBlock = blockedSensors.TryAdd(sensorId, true);

            if (isNewBlock)
            {
                Console.WriteLine($"[Server] Sensor {sensorId} has been added to the blocked list");
                return Ok($"Sensor {sensorId} successfully blocked");
            }

            return Ok($"Sensor {sensorId} was already in the blocked list");
        }
    }
}
