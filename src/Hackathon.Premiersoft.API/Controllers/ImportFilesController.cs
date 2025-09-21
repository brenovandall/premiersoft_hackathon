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

        /// <summary>
        /// Obtém lista de hospitais para seleção
        /// </summary>
        [HttpGet("hospitais/list")]
        public async Task<IActionResult> GetHospitalsList()
        {
            try
            {
                var hospitais = await _getDataService.GetHospitalsListAsync();
                return Ok(hospitais);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém detalhes específicos de um hospital
        /// </summary>
        [HttpGet("hospitais/{id}/details")]
        public async Task<IActionResult> GetHospitalDetails(int id)
        {
            try
            {
                var hospital = await _getDataService.GetHospitalDetailsAsync(id);
                if (hospital == null)
                {
                    return NotFound(new { message = "Hospital não encontrado" });
                }
                return Ok(hospital);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém especialidades médicas de um hospital específico
        /// </summary>
        [HttpGet("hospitais/{id}/especialidades")]
        public async Task<IActionResult> GetHospitalSpecialties(int id)
        {
            try
            {
                var especialidades = await _getDataService.GetHospitalSpecialtiesAsync(id);
                return Ok(especialidades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém dados demográficos de pacientes
        /// </summary>
        [HttpGet("pacientes/demographics")]
        public async Task<IActionResult> GetPatientDemographics()
        {
            try
            {
                var demographics = await _getDataService.GetPatientDemographicsAsync();
                return Ok(demographics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém estatísticas gerais de pacientes
        /// </summary>
        [HttpGet("pacientes/stats")]
        public async Task<IActionResult> GetPatientStats()
        {
            try
            {
                var stats = await _getDataService.GetPatientStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém estatísticas de médicos por especialidade
        /// </summary>
        [HttpGet("medicos/specialty-stats")]
        public async Task<IActionResult> GetDoctorSpecialtyStats()
        {
            try
            {
                var stats = await _getDataService.GetDoctorSpecialtyStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Busca médicos por nome e especialidade
        /// </summary>
        [HttpGet("medicos/search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string? searchTerm = null, [FromQuery] string? specialty = null)
        {
            try
            {
                var doctors = await _getDataService.SearchDoctorsAsync(searchTerm, specialty);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}