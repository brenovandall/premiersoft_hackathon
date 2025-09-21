using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Extensions;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services.ImportFiles;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ImportFilesController : ControllerBase
    {
        private readonly IImportFilesService _importFilesService;
        private IFileReaderEngineFactory FileReaderEngineFactory { get; set; }

        public ImportFilesController(IImportFilesService importFilesService)
        {
            _importFilesService = importFilesService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImportFilesRequest request, CancellationToken cancellationToken)
        {
            var import = await _importFilesService.Create(request, cancellationToken);
 

            return Ok(new 
            { 
                id = import.Id,
                fileName = import.FileName,
                dataType = import.DataType,
                fileFormat = import.FileFormat,
                status = import.Status,
                importedOn = import.ImportedOn,
                message = "Import criado com sucesso"
            });
        }

        [HttpGet]
        public ActionResult<IList<Import>> Get()
        {
            var imports = _importFilesService.GetAll();

            return Ok(imports);
        }
    }
}