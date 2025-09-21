using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines;
using Hackathon.Premiersoft.API.Engines.Extensions;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Xml;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Enums;
using Hackathon.Premiersoft.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class XmlFileReadController : ControllerBase
    {
        private IFileReaderEngineFactory Factory { get; set; }
        IRepository<Import, Guid> ImportRepository { get; set; }
        public XmlFileReadController(IFileReaderEngineFactory factory, IRepository<Import, Guid> importRepo)
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
                var import = Import.Create(
                dataType: ImportDataTypes.City,
                fileFormat: ImportFileFormat.Xml,
                fileName: "pacientes_2025_09_20.xml",
                s3PreSignedUrl: "uploads/municipios/2025-09-21/1758428070712-teste_municipios.xml",
                totalRegisters: 2,
                totalImportedRegisters: 110,
                totalDuplicatedRegisters: 5,
                totalFailedRegisters: 5,
                importedOn: DateTime.UtcNow.AddMinutes(-30),
                finishedOn: DateTime.UtcNow,
                description: "Arquivo de importação de pacientes de setembro/2025");

                ImportRepository.Add(import);

                var factory = Factory.CreateFactory(".xml");
                factory.Run(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

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
