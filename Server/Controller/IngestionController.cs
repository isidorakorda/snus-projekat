using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using Server.Features.Commands;

namespace Server.Controller
{
    [ApiController]
    [Route("api/ingestion")]
    public class IngestionController : ControllerBase
    {
        private readonly IMediator mediator;

        public IngestionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("ingest")]
        public async Task<IActionResult> Ingest([FromBody] IngestDataDTO dto)
        {
            if (dto == null) return BadRequest("Not valid");

            var success = await mediator.Send(new IngestDataCommand(dto));

            if (success)
            {
                return Ok(new { message = "Data successfully processed and saved" });
            }

            return StatusCode(500, "Internal Server Error occurred while trying to save data");
        }
    }
}
