using LabScheduler.Domain.Entities;
using LabScheduler.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabScheduler.Api.Controllers
{
    [ApiController]
    [Route("api/alertas")]
    public class AlertasController(AlertaEstoqueService service) : ControllerBase
    {
        private readonly AlertaEstoqueService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<AlertaEstoque>>> GetAll([FromQuery] AlertaStatus? status)
        {
            return Ok(await _service.GetAllAsync(status));
        }

        [HttpPost("{id}/resolver")]
        public async Task<ActionResult> Resolver(Guid id, [FromBody] ResolverAlertaRequest request)
        {
            try
            {
                await _service.ResolverAsync(id, request.AcaoTomada, request.Responsavel);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("ativos/count")]
        public async Task<ActionResult<int>> GetAlertasAtivosCount()
        {
            return Ok(await _service.GetAlertasAtivosCountAsync());
        }
    }

    public record ResolverAlertaRequest(string? AcaoTomada, string? Responsavel);
}
