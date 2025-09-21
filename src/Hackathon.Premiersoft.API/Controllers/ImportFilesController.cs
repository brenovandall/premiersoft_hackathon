using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services.ImportFiles;
using Hackathon.Premiersoft.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ImportFilesController : ControllerBase
    {
        private readonly IImportFilesService _importFilesService;
        private readonly IGetDataService _getDataService;

        public ImportFilesController(IImportFilesService importFilesService, IGetDataService getDataService)
        {
            _importFilesService = importFilesService;
            _getDataService = getDataService;
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

        [HttpGet("test-endpoint")]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "Endpoint de teste no ImportFiles funcionando" });
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                message = "API está funcionando corretamente"
            });
        }

        /// <summary>
        /// Obtém todos os estados
        /// </summary>
        [HttpGet("estados")]
        public async Task<IActionResult> GetEstados()
        {
            try
            {
                var estados = await _getDataService.GetEstadosAsync();
                return Ok(estados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os municípios
        /// </summary>
        [HttpGet("municipios")]
        public async Task<IActionResult> GetMunicipios()
        {
            try
            {
                var municipios = await _getDataService.GetMunicipiosAsync();
                return Ok(municipios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os pacientes
        /// </summary>
        [HttpGet("pacientes")]
        public async Task<IActionResult> GetPacientes()
        {
            try
            {
                var pacientes = await _getDataService.GetPacientesAsync();
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os médicos
        /// </summary>
        [HttpGet("medicos")]
        public async Task<IActionResult> GetMedicos()
        {
            try
            {
                var medicos = await _getDataService.GetMedicosAsync();
                return Ok(medicos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os hospitais
        /// </summary>
        [HttpGet("hospitais")]
        public async Task<IActionResult> GetHospitais()
        {
            try
            {
                var hospitais = await _getDataService.GetHospitaisAsync();
                return Ok(hospitais);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}