using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientAllocationController : ControllerBase
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;

        public PatientAllocationController(IPremiersoftHackathonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Lista todas as alocações de pacientes com filtros opcionais
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientAllocationDto>>> GetPatientAllocations(
            [FromQuery] string? regiao = null,
            [FromQuery] string? hospital = null,
            [FromQuery] string? especialidade = null,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = _dbContext.PatientsHospitals
                    .Include(ph => ph.Patient)
                        .ThenInclude(p => p.Cid10)
                    .Include(ph => ph.Patient)
                        .ThenInclude(p => p.Codigo_Municipio)
                    .Include(ph => ph.Hospital)
                        .ThenInclude(h => h.Municipio)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(regiao))
                {
                    query = query.Where(ph => ph.Patient.Codigo_Municipio.Codigo_uf.Contains(regiao));
                }

                if (!string.IsNullOrEmpty(hospital))
                {
                    query = query.Where(ph => ph.Hospital.Nome.Contains(hospital, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(especialidade))
                {
                    query = query.Where(ph => ph.Hospital.Especialidades.Contains(especialidade, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(ph => ph.Status == status);
                }

                // Paginação
                var totalItems = await query.CountAsync();
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(ph => ph.DataAlocacao)
                    .ToListAsync();

                var result = items.Select(ph => new PatientAllocationDto
                {
                    Id = ph.Id,
                    PacienteNome = ph.Patient.Nome_completo,
                    PacienteCpf = ph.Patient.Cpf,
                    CidCodigo = ph.Patient.Cid10.Codigo,
                    CidDescricao = ph.Patient.Cid10.Descricao,
                    HospitalNome = ph.Hospital.Nome,
                    HospitalCodigo = ph.Hospital.Codigo,
                    HospitalEspecialidades = ph.Hospital.Especialidades,
                    DistanciaKm = Math.Round(ph.Distancia, 2),
                    Status = ph.Status,
                    DataAlocacao = ph.DataAlocacao,
                    Prioridade = ph.Prioridade,
                    Observacoes = ph.Observacoes,
                    PacienteMunicipio = ph.Patient.Codigo_Municipio.Nome,
                    HospitalMunicipio = ph.Hospital.Municipio?.Nome ?? "",
                    Regiao = ph.Patient.Codigo_Municipio.Codigo_uf
                }).ToList();

                return Ok(new
                {
                    Items = result,
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno do servidor", Error = ex.Message });
            }
        }

        /// <summary>
        /// Busca alocações de um paciente específico
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<PatientAllocationDto>>> GetPatientAllocationsById(Guid patientId)
        {
            try
            {
                var allocations = await _dbContext.PatientsHospitals
                    .Include(ph => ph.Patient)
                        .ThenInclude(p => p.Cid10)
                    .Include(ph => ph.Patient)
                        .ThenInclude(p => p.Codigo_Municipio)
                    .Include(ph => ph.Hospital)
                        .ThenInclude(h => h.Municipio)
                    .Where(ph => ph.PatientId == patientId)
                    .OrderByDescending(ph => ph.DataAlocacao)
                    .ToListAsync();

                var result = allocations.Select(ph => new PatientAllocationDto
                {
                    Id = ph.Id,
                    PacienteNome = ph.Patient.Nome_completo,
                    PacienteCpf = ph.Patient.Cpf,
                    CidCodigo = ph.Patient.Cid10.Codigo,
                    CidDescricao = ph.Patient.Cid10.Descricao,
                    HospitalNome = ph.Hospital.Nome,
                    HospitalCodigo = ph.Hospital.Codigo,
                    HospitalEspecialidades = ph.Hospital.Especialidades,
                    DistanciaKm = Math.Round(ph.Distancia, 2),
                    Status = ph.Status,
                    DataAlocacao = ph.DataAlocacao,
                    Prioridade = ph.Prioridade,
                    Observacoes = ph.Observacoes,
                    PacienteMunicipio = ph.Patient.Codigo_Municipio.Nome,
                    HospitalMunicipio = ph.Hospital.Municipio?.Nome ?? "",
                    Regiao = ph.Patient.Codigo_Municipio.Codigo_uf
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno do servidor", Error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza o status de uma alocação
        /// </summary>
        [HttpPut("{allocationId}/status")]
        public async Task<ActionResult> UpdateAllocationStatus(Guid allocationId, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var allocation = await _dbContext.PatientsHospitals.FindAsync(allocationId);
                if (allocation == null)
                {
                    return NotFound(new { Message = "Alocação não encontrada" });
                }

                allocation.Status = request.NovoStatus;
                allocation.Observacoes = request.Observacoes;

                await _dbContext.SaveChangesAsync(CancellationToken.None);

                return Ok(new { Message = "Status atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno do servidor", Error = ex.Message });
            }
        }

        /// <summary>
        /// Estatísticas das alocações
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult> GetAllocationStatistics()
        {
            try
            {
                var totalAllocations = await _dbContext.PatientsHospitals.CountAsync();
                var activeAllocations = await _dbContext.PatientsHospitals.CountAsync(ph => ph.Status == "Alocado");
                var transferredAllocations = await _dbContext.PatientsHospitals.CountAsync(ph => ph.Status == "Transferido");
                var dischargedAllocations = await _dbContext.PatientsHospitals.CountAsync(ph => ph.Status == "Alta");

                var avgDistance = await _dbContext.PatientsHospitals.AverageAsync(ph => ph.Distancia);

                var topHospitals = await _dbContext.PatientsHospitals
                    .Include(ph => ph.Hospital)
                    .GroupBy(ph => ph.Hospital.Nome)
                    .Select(g => new { Hospital = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToListAsync();

                return Ok(new
                {
                    TotalAlocacoes = totalAllocations,
                    AlocacoesAtivas = activeAllocations,
                    Transferencias = transferredAllocations,
                    Altas = dischargedAllocations,
                    DistanciaMediaKm = Math.Round(avgDistance, 2),
                    TopHospitais = topHospitals
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno do servidor", Error = ex.Message });
            }
        }
    }

    public class UpdateStatusRequest
    {
        public string NovoStatus { get; set; } = default!;
        public string? Observacoes { get; set; }
    }
}