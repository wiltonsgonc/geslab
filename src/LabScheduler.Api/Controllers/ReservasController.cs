using LabScheduler.Domain.DTOs;
using LabScheduler.Domain.Entities;
using LabScheduler.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabScheduler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController(ReservaService service) : ControllerBase
    {
        private readonly ReservaService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<ReservaDto>>> GetAll([FromQuery] DateTime? data)
        {
            return Ok(await _service.GetAllAsync(data));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservaDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("calendario")]
        public async Task<ActionResult<List<ReservaCalendarioDto>>> GetCalendario(
            [FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            return Ok(await _service.GetCalendarioAsync(inicio, fim));
        }

        [HttpGet("horarios-disponiveis")]
        public async Task<ActionResult<List<TimeSpan>>> GetHorariosDisponiveis(
            [FromQuery] DateTime data, [FromQuery] Guid? equipamentoId)
        {
            return Ok(await _service.GetHorariosDisponiveisAsync(data, equipamentoId));
        }

        [HttpPost]
        public async Task<ActionResult<ReservaDto>> Create(CreateReservaDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ReservaDto>> UpdateStatus(Guid id, UpdateReservaStatusDto dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            return Ok(await _service.GetDashboardAsync());
        }
    }
}
