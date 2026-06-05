using LabScheduler.Domain.DTOs;
using LabScheduler.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabScheduler.Api.Controllers
{
    [ApiController]
    [Route("api/movimentos-estoque")]
    public class MovimentosEstoqueController(MovimentoEstoqueService service) : ControllerBase
    {
        private readonly MovimentoEstoqueService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<MovimentoEstoqueDto>>> GetAll([FromQuery] Guid? insumoId)
        {
            return Ok(await _service.GetAllAsync(insumoId));
        }

        [HttpPost]
        public async Task<ActionResult<MovimentoEstoqueDto>> Create(CreateMovimentoDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { insumoId = dto.InsumoId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
