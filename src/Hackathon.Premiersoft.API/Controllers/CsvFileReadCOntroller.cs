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

        
    }
}
