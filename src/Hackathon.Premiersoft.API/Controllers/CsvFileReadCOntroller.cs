using Hackathon.Premiersoft.API.Engines;
using Hackathon.Premiersoft.API.Engines.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CsvFileReadController : ControllerBase
    {
        [HttpGet]
        [HttpGet("ler")]  
        public IActionResult GetCsvFileData()
        {
            var csvReader = new CsvFileReaderEngine(); 

            // Substitua pelo caminho do seu arquivo
            string filePath = @"C:\Users\DELL\Downloads\estados.csv";

            csvReader.Run(filePath);
            // Add your CSV reading logic here
            return Ok("CSV data read successfully");
        }
    }
}
