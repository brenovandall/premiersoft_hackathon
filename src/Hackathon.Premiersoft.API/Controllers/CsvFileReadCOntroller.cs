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

          
            // Add your CSV reading logic here
            return Ok("CSV data read successfully");
        }


        public class RespostaOk
        {
            public string Status { get; set; }
        }

        [HttpGet]
        [HttpGet("ok")]
        public IActionResult RetornaOk()
        {
            var resposta = new RespostaOk { Status = "ok" };
            return Ok(resposta);
        }
    }
}
