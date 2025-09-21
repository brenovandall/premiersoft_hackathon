using Hackathon.Premiersoft.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Cid10EspecialidadeController : ControllerBase
    {
        private readonly Cid10EspecialidadeService _cid10EspecialidadeService;

        public Cid10EspecialidadeController(Cid10EspecialidadeService cid10EspecialidadeService)
        {
            _cid10EspecialidadeService = cid10EspecialidadeService;
        }

        /// <summary>
        /// Popula a tabela de mapeamentos CID-10 x Especialidade com dados iniciais
        /// </summary>
        [HttpPost("popular-mapeamentos")]
        public async Task<ActionResult> PopularMapeamentos()
        {
            try
            {
                await _cid10EspecialidadeService.PopularMapeamentosCid10EspecialidadeAsync();
                return Ok(new { Message = "Mapeamentos CID-10 x Especialidade populados com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao popular mapeamentos", Error = ex.Message });
            }
        }

        /// <summary>
        /// Busca especialidades recomendadas para um código CID-10
        /// </summary>
        [HttpGet("especialidades/{cidCodigo}")]
        public async Task<ActionResult<List<string>>> BuscarEspecialidades(string cidCodigo)
        {
            try
            {
                var especialidades = await _cid10EspecialidadeService.BuscarEspecialidadesPorCidAsync(cidCodigo);
                
                if (!especialidades.Any())
                {
                    return Ok(new List<string> { "Clínica Geral" }); // Fallback
                }

                return Ok(especialidades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao buscar especialidades", Error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém estatísticas dos mapeamentos CID-Especialidade
        /// </summary>
        [HttpGet("estatisticas")]
        public async Task<ActionResult> ObterEstatisticas()
        {
            try
            {
                var estatisticas = await _cid10EspecialidadeService.ObterEstatisticasMapeamentosAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao obter estatísticas", Error = ex.Message });
            }
        }
    }
}