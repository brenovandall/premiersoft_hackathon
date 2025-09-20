using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines;
using Hackathon.Premiersoft.API.Engines.Extensions;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Xml;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class XmlFileReadController : ControllerBase
    {
        private IFileReaderEngineFactory Factory { get; set; }
        IRepository<Import, long> ImportRepository { get; set; }
        public XmlFileReadController(IFileReaderEngineFactory factory, IRepository<Import, long> importRepo)
        {
            Factory = factory;
            ImportRepository = importRepo;
        }

        [HttpGet]
        [HttpGet("ler")]
        public IActionResult GetXmlData()
        {
            try
            {
                var factory = Factory.CreateFactory(".xml");
                factory.Run(1);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

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
