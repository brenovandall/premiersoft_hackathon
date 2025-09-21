using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class TestUploadController : ControllerBase
    {
        public class UploadResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public object? Data { get; set; }
        }

        [HttpOptions("test")]
        public IActionResult TestOptions()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept");
            return Ok();
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestUpload()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var jsonString = await reader.ReadToEndAsync();
                
                Console.WriteLine("========================================");
                Console.WriteLine("📥 DADOS DO UPLOAD RECEBIDOS:");
                Console.WriteLine("========================================");
                Console.WriteLine($"📄 Raw JSON: {jsonString}");
                
                // Parse do JSON
                var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonString);
                var root = jsonDoc.RootElement;
                
                if (root.TryGetProperty("fileName", out var fileNameElement))
                {
                    Console.WriteLine($"📁 Nome do Arquivo: {fileNameElement.GetString()}");
                }
                
                if (root.TryGetProperty("dataType", out var dataTypeElement))
                {
                    Console.WriteLine($"🏷️  Tipo de Dados: {dataTypeElement.GetString()}");
                }
                
                if (root.TryGetProperty("fieldMappings", out var fieldMappingsElement))
                {
                    Console.WriteLine($"🗂️  MAPEAMENTOS DE CAMPOS:");
                    int counter = 1;
                    foreach (var mapping in fieldMappingsElement.EnumerateArray())
                    {
                        foreach (var property in mapping.EnumerateObject())
                        {
                            Console.WriteLine($"   {counter:D2}. {property.Name} → {property.Value.GetString()}");
                            counter++;
                        }
                    }
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
                        Status = "processed",
                        FieldMappings = root.TryGetProperty("fieldMappings", out var mappings) ? mappings : (object)"no mappings"
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