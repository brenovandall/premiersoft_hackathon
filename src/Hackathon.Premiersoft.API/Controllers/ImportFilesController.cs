using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Services.ImportFiles;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImportFilesController : ControllerBase
    {
        private readonly IImportFilesService _importFilesService;

        public ImportFilesController(IImportFilesService importFilesService)
        {
            _importFilesService = importFilesService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImportFilesRequest request, CancellationToken cancellationToken)
        {
            await _importFilesService.Create(request, cancellationToken);

            return Created();
        }
    }
}