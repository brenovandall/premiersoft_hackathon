using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class DirectImportController : ControllerBase
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;

        public DirectImportController(IPremiersoftHackathonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public class DirectImportRequest
        {
            public int DataType { get; set; }
            public int FileFormat { get; set; }
            public string FileName { get; set; } = string.Empty;
            public string FileContent { get; set; } = string.Empty; // Conteúdo do arquivo em base64 ou texto
        }

        public class DirectImportResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public int ProcessedRecords { get; set; }
            public int SuccessfulRecords { get; set; }
            public int ErrorRecords { get; set; }
            public List<string> Errors { get; set; } = new();
        }

        [HttpPost("estados")]
        public async Task<IActionResult> ImportEstados([FromBody] DirectImportRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (request.DataType != (int)ImportDataTypes.State)
                {
                    return BadRequest("Este endpoint é específico para importação de estados");
                }

                var response = new DirectImportResponse();
                var errors = new List<string>();

                // Decodificar o conteúdo do arquivo (assumindo que é texto CSV)
                string csvContent;
                try
                {
                    var bytes = Convert.FromBase64String(request.FileContent);
                    csvContent = System.Text.Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    // Se não for base64, assumir que é texto direto
                    csvContent = request.FileContent;
                }

                // Processar CSV
                var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                if (lines.Length < 2)
                {
                    return BadRequest("Arquivo CSV deve ter pelo menos um cabeçalho e uma linha de dados");
                }

                // Pular cabeçalho
                var dataLines = lines.Skip(1);
                var processedCount = 0;
                var successCount = 0;

                foreach (var line in dataLines)
                {
                    processedCount++;
                    try
                    {
                        var columns = line.Split(',');
                        
                        if (columns.Length < 6)
                        {
                            errors.Add($"Linha {processedCount}: Número insuficiente de colunas");
                            continue;
                        }

                        // Mapear colunas do CSV: codigo_uf,uf,nome,latitude,longitude,regiao
                        var estado = new Estados
                        {
                            Id = Guid.NewGuid(),
                            Codigo_uf = int.TryParse(columns[0], out var codigo) ? codigo : 0,
                            Uf = columns[1].Trim().Trim('"'),
                            Nome = columns[2].Trim().Trim('"'),
                            Latitude = decimal.TryParse(columns[3], out var lat) ? lat : 0,
                            Longitude = decimal.TryParse(columns[4], out var lng) ? lng : 0,
                            Regiao = columns[5].Trim().Trim('"')
                        };

                        // Verificar se já existe
                        var existeEstado = await _dbContext.Estados
                            .FirstOrDefaultAsync(e => e.Codigo_uf == estado.Codigo_uf, cancellationToken);

                        if (existeEstado == null)
                        {
                            _dbContext.Estados.Add(estado);
                            successCount++;
                        }
                        else
                        {
                            // Atualizar dados existentes
                            existeEstado.Uf = estado.Uf;
                            existeEstado.Nome = estado.Nome;
                            existeEstado.Latitude = estado.Latitude;
                            existeEstado.Longitude = estado.Longitude;
                            existeEstado.Regiao = estado.Regiao;
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Linha {processedCount}: {ex.Message}");
                    }
                }

                // Salvar no banco
                await _dbContext.SaveChangesAsync(cancellationToken);

                response.Success = true;
                response.Message = $"Importação concluída: {successCount} estados processados com sucesso";
                response.ProcessedRecords = processedCount;
                response.SuccessfulRecords = successCount;
                response.ErrorRecords = errors.Count;
                response.Errors = errors;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DirectImportResponse
                {
                    Success = false,
                    Message = $"Erro interno: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("municipios")]
        public async Task<IActionResult> ImportMunicipios([FromBody] DirectImportRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (request.DataType != (int)ImportDataTypes.City)
                {
                    return BadRequest("Este endpoint é específico para importação de municípios");
                }

                var response = new DirectImportResponse();
                var errors = new List<string>();

                // Decodificar o conteúdo do arquivo
                string csvContent;
                try
                {
                    var bytes = Convert.FromBase64String(request.FileContent);
                    csvContent = System.Text.Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    csvContent = request.FileContent;
                }

                var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                if (lines.Length < 2)
                {
                    return BadRequest("Arquivo CSV deve ter pelo menos um cabeçalho e uma linha de dados");
                }

                var dataLines = lines.Skip(1);
                var processedCount = 0;
                var successCount = 0;

                foreach (var line in dataLines)
                {
                    processedCount++;
                    try
                    {
                        var columns = line.Split(',');
                        
                        if (columns.Length < 7)
                        {
                            errors.Add($"Linha {processedCount}: Número insuficiente de colunas");
                            continue;
                        }

                        // Assumindo formato: codigo_ibge,nome,latitude,longitude,capital,codigo_uf,siafi_id
                        var municipio = new Municipios
                        {
                            Id = Guid.NewGuid(),
                            Codigo_ibge = columns[0].Trim().Trim('"'),
                            Nome = columns[1].Trim().Trim('"'),
                            Latitude = decimal.TryParse(columns[2], out var lat) ? lat : 0,
                            Longitude = decimal.TryParse(columns[3], out var lng) ? lng : 0,
                            Capital = bool.TryParse(columns[4], out var cap) ? cap : false,
                            Codigo_uf = columns[5].Trim().Trim('"'),
                            Siafi_id = int.TryParse(columns[6], out var siafi) ? siafi : 0,
                            Ddd = 0, // Não disponível no CSV básico
                            Fuso_horario = "America/Sao_Paulo", // Padrão
                            Populacao = 0, // Não disponível no CSV básico
                            Erros = ""
                        };

                        var existeMunicipio = await _dbContext.Cidades
                            .FirstOrDefaultAsync(m => m.Codigo_ibge == municipio.Codigo_ibge, cancellationToken);

                        if (existeMunicipio == null)
                        {
                            _dbContext.Cidades.Add(municipio);
                            successCount++;
                        }
                        else
                        {
                            // Atualizar dados existentes
                            existeMunicipio.Nome = municipio.Nome;
                            existeMunicipio.Codigo_uf = municipio.Codigo_uf;
                            existeMunicipio.Latitude = municipio.Latitude;
                            existeMunicipio.Longitude = municipio.Longitude;
                            existeMunicipio.Capital = municipio.Capital;
                            existeMunicipio.Siafi_id = municipio.Siafi_id;
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Linha {processedCount}: {ex.Message}");
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                response.Success = true;
                response.Message = $"Importação concluída: {successCount} municípios processados com sucesso";
                response.ProcessedRecords = processedCount;
                response.SuccessfulRecords = successCount;
                response.ErrorRecords = errors.Count;
                response.Errors = errors;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DirectImportResponse
                {
                    Success = false,
                    Message = $"Erro interno: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}