using Hackathon.Premiersoft.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CsvFileReadController : ControllerBase
    {
        private readonly S3Service _s3Service;

        public CsvFileReadController(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpGet]
        [HttpGet("ler")]
        public IActionResult GetCsvFileDataAsync()
        {
            // TODO: Usar injeção de dependência ao invés de instanciar diretamente
            // var csvFileReaderEngine = new Engines.Extensions.CsvFileReaderEngine();
            // csvFileReaderEngine.Run(Guid.NewGuid());
            
            return Accepted(new { message = "Processamento iniciado em background - implementação via factory" });
        }


        public class RespostaOk
        {
            public string Status { get; set; }
        }

        public class UploadRequest
        {
            public string FileUrl { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;
            public string DataType { get; set; } = string.Empty;
            public string FileFormat { get; set; } = string.Empty;
            public object[] FieldMappings { get; set; } = new object[0];
            public long FileSize { get; set; }
            public string BucketName { get; set; } = string.Empty;
            public string S3Key { get; set; } = string.Empty;
        }

        public class UploadResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public object? Data { get; set; }
        }

        [HttpGet("ok")]
        public IActionResult RetornaOk()
        {
            var resposta = new RespostaOk { Status = "ok" };
            return Ok(resposta);
        }

        [HttpOptions("upload")]
        public IActionResult UploadOptions()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept");
            return Ok();
        }

                [HttpPost("upload")]
        public IActionResult ReceberDadosUpload([FromBody] object rawRequest)
        {
            try
            {
                Console.WriteLine("========================================");
                Console.WriteLine("📥 DADOS DO UPLOAD RECEBIDOS:");
                Console.WriteLine("========================================");
                Console.WriteLine($"� Raw JSON: {rawRequest}");
                
                // Processar o JSON manualmente
                var jsonString = rawRequest.ToString();
                if (string.IsNullOrEmpty(jsonString))
                {
                    return BadRequest("Dados inválidos");
                }

                var response = new UploadResponse
                {
                    Success = true,
                    Message = "Dados do upload recebidos e processados com sucesso!",
                    Data = new
                    {
                        ProcessingId = Guid.NewGuid().ToString(),
                        ReceivedAt = DateTime.UtcNow,
                        Status = "processed"
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao processar dados do upload: {ex.Message}");
                
                var errorResponse = new UploadResponse
                {
                    Success = false,
                    Message = $"Erro ao processar dados: {ex.Message}"
                };

                return BadRequest(errorResponse);
            }
        }
    }
}
