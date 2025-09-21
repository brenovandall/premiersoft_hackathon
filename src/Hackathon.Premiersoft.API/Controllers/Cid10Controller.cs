using Hackathon.Premiersoft.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class Cid10Controller : ControllerBase
    {
        private readonly ICid10ImportService _cid10ImportService;

        public Cid10Controller(ICid10ImportService cid10ImportService)
        {
            _cid10ImportService = cid10ImportService;
        }

        /// <summary>
        /// Importa códigos CID-10 a partir de um arquivo CSV
        /// </summary>
        /// <param name="file">Arquivo CSV contendo códigos CID-10</param>
        /// <returns>Resultado da importação</returns>
        [HttpPost("import")]
        public async Task<IActionResult> ImportCid10FromFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Arquivo não fornecido ou está vazio." });
                }

                // Verificar se é um arquivo CSV
                var allowedExtensions = new[] { ".csv", ".txt" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Apenas arquivos CSV (.csv) ou texto (.txt) são permitidos." });
                }

                using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                var result = await _cid10ImportService.ImportFromReaderAsync(reader);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Importação concluída com sucesso.",
                        summary = result.GetSummary(),
                        details = new
                        {
                            processedCount = result.ProcessedCount,
                            importedCount = result.ImportedCount,
                            skippedCount = result.SkippedCount
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Falha na importação.",
                        error = result.ErrorMessage,
                        summary = result.GetSummary()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Importa códigos CID-10 a partir de dados em texto
        /// </summary>
        /// <param name="request">Dados CID-10 em formato texto</param>
        /// <returns>Resultado da importação</returns>
        [HttpPost("import-text")]
        public async Task<IActionResult> ImportCid10FromText([FromBody] Cid10ImportTextRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Data))
                {
                    return BadRequest(new { message = "Dados não fornecidos." });
                }

                using var reader = new StringReader(request.Data);
                var result = await _cid10ImportService.ImportFromReaderAsync(reader);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Importação concluída com sucesso.",
                        summary = result.GetSummary(),
                        details = new
                        {
                            processedCount = result.ProcessedCount,
                            importedCount = result.ImportedCount,
                            skippedCount = result.SkippedCount
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Falha na importação.",
                        error = result.ErrorMessage,
                        summary = result.GetSummary()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Retorna estatísticas sobre os códigos CID-10 importados
        /// </summary>
        /// <returns>Estatísticas dos códigos CID-10</returns>
        [HttpGet("stats")]
        public IActionResult GetCid10Stats()
        {
            try
            {
                // Aqui você pode adicionar lógica para buscar estatísticas do banco
                // Por enquanto, retornando uma resposta básica
                return Ok(new
                {
                    message = "Endpoint de estatísticas CID-10",
                    note = "Implementar lógica de busca no banco de dados conforme necessário"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor.",
                    error = ex.Message
                });
            }
        }
    }

    public class Cid10ImportTextRequest
    {
        public string Data { get; set; } = string.Empty;
    }
}