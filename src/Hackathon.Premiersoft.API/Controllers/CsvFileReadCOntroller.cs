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

            var csvFileReaderEngine = new Engines.Extensions.CsvFileReaderEngine();
            csvFileReaderEngine.Run(0);
            

            return Accepted(new { message = "Processamento iniciado em background" });
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
            public Dictionary<string, string> FieldMappings { get; set; } = new Dictionary<string, string>();
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
        public IActionResult ReceberDadosUpload([FromBody] UploadRequest request)
        {
            try
            {
                Console.WriteLine("========================================");
                Console.WriteLine("📥 DADOS DO UPLOAD RECEBIDOS:");
                Console.WriteLine("========================================");
                Console.WriteLine($"📁 Nome do Arquivo: {request.FileName}");
                Console.WriteLine($"🏷️  Tipo de Dados: {request.DataType}");
                Console.WriteLine($"📄 Formato: {request.FileFormat}");
                Console.WriteLine($"📊 Tamanho: {request.FileSize:N0} bytes ({request.FileSize / 1024.0 / 1024.0:F2} MB)");
                Console.WriteLine($"📦 Bucket S3: {request.BucketName}");
                Console.WriteLine($"🔑 Chave S3: {request.S3Key}");
                Console.WriteLine($"🔗 URL do Arquivo:");
                Console.WriteLine($"   {request.FileUrl}");
                Console.WriteLine();
                Console.WriteLine($"🗂️  MAPEAMENTOS DE CAMPOS ({request.FieldMappings.Count} itens):");
                
                if (request.FieldMappings.Any())
                {
                    int counter = 1;
                    foreach (var mapping in request.FieldMappings)
                    {
                        Console.WriteLine($"   {counter:D2}. {mapping.Key} → {mapping.Value}");
                        counter++;
                    }
                }
                else
                {
                    Console.WriteLine("   (Nenhum mapeamento configurado)");
                }
                
                Console.WriteLine();
                Console.WriteLine("✅ Dados recebidos e processados com sucesso!");
                Console.WriteLine("========================================");

                var response = new UploadResponse
                {
                    Success = true,
                    Message = "Dados do upload recebidos e processados com sucesso!",
                    Data = new
                    {
                        ProcessingId = Guid.NewGuid().ToString(),
                        ReceivedAt = DateTime.UtcNow,
                        FileName = request.FileName,
                        DataType = request.DataType,
                        MappingsCount = request.FieldMappings.Count,
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
