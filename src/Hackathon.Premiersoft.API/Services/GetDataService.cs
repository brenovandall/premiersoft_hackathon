using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Services
{
    public interface IGetDataService
    {
        Task<IEnumerable<EstadosDto>> GetEstadosAsync();
        Task<IEnumerable<MunicipiosDto>> GetMunicipiosAsync();
        Task<IEnumerable<PacientesDto>> GetPacientesAsync();
        Task<IEnumerable<MedicosDto>> GetMedicosAsync();
        Task<IEnumerable<HospitaisDto>> GetHospitaisAsync();
        Task<IEnumerable<MunicipiosDto>> GetMunicipiosByEstadoAsync(string codigoUf);
        Task<IEnumerable<HospitalListDto>> GetHospitalsListAsync();
        Task<HospitalDetailsDto?> GetHospitalDetailsAsync(int hospitalId);
        Task<IEnumerable<HospitalSpecialtyDto>> GetHospitalSpecialtiesAsync(int hospitalId);
        Task<IEnumerable<PatientDemographicDto>> GetPatientDemographicsAsync();
        Task<PatientStatsDto> GetPatientStatsAsync();
        Task<IEnumerable<DoctorSpecialtyStatsDto>> GetDoctorSpecialtyStatsAsync();
        Task<IEnumerable<DoctorSearchDto>> SearchDoctorsAsync(string? searchTerm = null, string? specialty = null);
    }

    public class GetDataService : IGetDataService
    {
        private readonly IPremiersoftHackathonDbContext _context;

        public GetDataService(IPremiersoftHackathonDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EstadosDto>> GetEstadosAsync()
        {
            var estados = await _context.Estados
                .Select(e => new EstadosDto
                {
                    Id = e.Id,
                    Codigo_uf = e.Codigo_uf,
                    Uf = e.Uf,
                    Nome = e.Nome,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    Regiao = e.Regiao
                })
                .ToListAsync();

            return estados;
        }

        public async Task<IEnumerable<MunicipiosDto>> GetMunicipiosAsync()
        {
            var municipios = await _context.Cidades
                .Select(m => new MunicipiosDto
                {
                    Id = m.Id,
                    Codigo_ibge = m.Codigo_ibge,
                    Nome = m.Nome,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Capital = m.Capital,
                    Codigo_uf = m.Codigo_uf,
                    Siafi_id = m.Siafi_id,
                    Ddd = m.Ddd,
                    Fuso_horario = m.Fuso_horario,
                    Populacao = m.Populacao,
                })
                .ToListAsync();

            return municipios;
        }

        public async Task<IEnumerable<MunicipiosDto>> GetMunicipiosByEstadoAsync(string codigoUf)
        {
            var municipios = await _context.Cidades
                .Where(m => m.Codigo_uf == codigoUf)
                .Select(m => new MunicipiosDto
                {
                    Id = m.Id,
                    Codigo_ibge = m.Codigo_ibge,
                    Nome = m.Nome,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Capital = m.Capital,
                    Codigo_uf = m.Codigo_uf,
                    Siafi_id = m.Siafi_id,
                    Ddd = m.Ddd,
                    Fuso_horario = m.Fuso_horario,
                    Populacao = m.Populacao,
                  
                })
                .ToListAsync();

            return municipios;
        }

        public async Task<IEnumerable<PacientesDto>> GetPacientesAsync()
        {
            var pacientes = await _context.Pacientes
                .Include(p => p.Codigo_Municipio)
                .Include(p => p.Cid10)
                .Select(p => new PacientesDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Cpf = p.Cpf,
                    Genero = p.Genero,
                    Nome_completo = p.Nome_completo,
                    Convenio = p.Convenio,
                    Municipio = p.Codigo_Municipio.Nome,
                    Cid10 = p.Cid10.Codigo,
                    DescricaoCid10 = p.Cid10.Descricao
                })
                .ToListAsync();

            return pacientes;
        }

        public async Task<IEnumerable<MedicosDto>> GetMedicosAsync()
        {
            var medicos = await _context.Medicos
                .Include(m => m.Codigo_Municipio)
                .Select(m => new MedicosDto
                {
                    Id = m.Id,
                    Codigo = m.Codigo,
                    Nome_completo = m.Nome_completo,
                    Especialidade = m.Especialidade,
                    Municipio = m.Codigo_Municipio.Nome
                })
                .ToListAsync();

            return medicos;
        }

        public async Task<IEnumerable<HospitaisDto>> GetHospitaisAsync()
        {
            var hospitais = await _context.Hospitais
                .Include(h => h.Cidade)
                .Select(h => new HospitaisDto
                {
                    Id = h.Id,
                    Codigo = h.Codigo,
                    Nome = h.Nome,
                    Bairro = h.Bairro,
                    Cidade = h.Cidade.Nome,
                    Leitos_totais = h.Leitos_totais
                })
                .ToListAsync();

            return hospitais;
        }

        public async Task<IEnumerable<HospitalListDto>> GetHospitalsListAsync()
        {
            var hospitais = await _context.Hospitais
                .Include(h => h.Cidade)
                .Select(h => new HospitalListDto
                {
                    Id = (int)h.Id.GetHashCode(), // Conversão temporária para int
                    Nome = h.Nome,
                    Cidade = h.Cidade.Nome,
                    Uf = h.Cidade.Codigo_uf
                })
                .ToListAsync();

            return hospitais;
        }

        public async Task<HospitalDetailsDto?> GetHospitalDetailsAsync(int hospitalId)
        {
            var hospital = await _context.Hospitais
                .Include(h => h.Cidade)
                .Where(h => h.Id.GetHashCode() == hospitalId)
                .FirstOrDefaultAsync();

            if (hospital == null)
                return null;

            // Contar médicos alocados ao hospital (assumindo relação através da cidade)
            var medicosAlocados = await _context.Medicos
                .Where(m => m.Codigo_Municipio.Nome == hospital.Cidade.Nome)
                .CountAsync();

            // Contar pacientes do hospital (assumindo relação através da cidade)
            var pacientesHospital = await _context.Pacientes
                .Where(p => p.Codigo_Municipio.Nome == hospital.Cidade.Nome)
                .CountAsync();

            // Simular leitos ocupados (seria melhor ter uma tabela de ocupação)
            var ocupacaoSimulada = new Random().Next(60, 95);
            var leitosOcupados = (int)(hospital.Leitos_totais * ocupacaoSimulada / 100.0);

            return new HospitalDetailsDto
            {
                Id = hospitalId,
                Nome = hospital.Nome,
                Cidade = hospital.Cidade.Nome,
                Estado = "Estado", // Temporário - precisaria buscar do estado
                Uf = hospital.Cidade.Codigo_uf,
                LeitosTotal = (int)hospital.Leitos_totais,
                LeitosOcupados = leitosOcupados,
                MedicosAlocados = medicosAlocados,
                TaxaOcupacao = ocupacaoSimulada,
                RankingRegional = new Random().Next(1, 10),
                TipoHospital = "Público" // Poderia vir de uma tabela
            };
        }

        public async Task<IEnumerable<HospitalSpecialtyDto>> GetHospitalSpecialtiesAsync(int hospitalId)
        {
            var hospital = await _context.Hospitais
                .Where(h => h.Id.GetHashCode() == hospitalId)
                .FirstOrDefaultAsync();

            if (hospital == null)
                return new List<HospitalSpecialtyDto>();

            // Buscar médicos por especialidade na região do hospital
            var especialidades = await _context.Medicos
                .Where(m => m.Codigo_Municipio.Nome == hospital.Cidade.Nome)
                .GroupBy(m => m.Especialidade)
                .Select(g => new HospitalSpecialtyDto
                {
                    Especialidade = g.Key,
                    NumeroMedicos = g.Count(),
                    NumeroPacientes = g.Count() * 25 // Estimativa de 25 pacientes por médico
                })
                .OrderByDescending(s => s.NumeroMedicos)
                .Take(6)
                .ToListAsync();

            return especialidades;
        }

        public async Task<IEnumerable<PatientDemographicDto>> GetPatientDemographicsAsync()
        {
            // Simulação de dados demográficos baseados nos pacientes reais
            // Em um cenário real, teríamos campos de data de nascimento nos pacientes
            var totalPacientes = await _context.Pacientes.CountAsync();
            
            if (totalPacientes == 0)
            {
                return new List<PatientDemographicDto>();
            }

            // Simular distribuição por idade e gênero
            var demographics = new List<PatientDemographicDto>
            {
                new() { AgeGroup = "0-18", Male = (int)(totalPacientes * 0.10), Female = (int)(totalPacientes * 0.09) },
                new() { AgeGroup = "19-35", Male = (int)(totalPacientes * 0.15), Female = (int)(totalPacientes * 0.16) },
                new() { AgeGroup = "36-50", Male = (int)(totalPacientes * 0.18), Female = (int)(totalPacientes * 0.17) },
                new() { AgeGroup = "51-65", Male = (int)(totalPacientes * 0.23), Female = (int)(totalPacientes * 0.21) },
                new() { AgeGroup = "65+", Male = (int)(totalPacientes * 0.28), Female = (int)(totalPacientes * 0.31) }
            };

            return demographics;
        }

        public async Task<PatientStatsDto> GetPatientStatsAsync()
        {
            var totalPacientes = await _context.Pacientes.CountAsync();
            var pacientesMasculinos = await _context.Pacientes.Where(p => p.Genero == "M").CountAsync();
            var pacientesFemininos = await _context.Pacientes.Where(p => p.Genero == "F").CountAsync();

            return new PatientStatsDto
            {
                TotalPatients = totalPacientes,
                MalePatients = pacientesMasculinos,
                FemalePatients = pacientesFemininos
            };
        }

        public async Task<IEnumerable<DoctorSpecialtyStatsDto>> GetDoctorSpecialtyStatsAsync()
        {
            var especialidades = await _context.Medicos
                .GroupBy(m => m.Especialidade)
                .Select(g => new DoctorSpecialtyStatsDto
                {
                    Specialty = g.Key,
                    Doctors = g.Count(),
                    Patients = g.Count() * 25 // Estimativa de 25 pacientes por médico
                })
                .OrderByDescending(s => s.Doctors)
                .Take(10)
                .ToListAsync();

            return especialidades;
        }

        public async Task<IEnumerable<DoctorSearchDto>> SearchDoctorsAsync(string? searchTerm = null, string? specialty = null)
        {
            var query = _context.Medicos.Include(m => m.Codigo_Municipio).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(m => m.Nome_completo.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(specialty) && specialty != "all")
            {
                query = query.Where(m => m.Especialidade == specialty);
            }

            var medicos = await query
                .Take(50) // Limitar resultados
                .Select(m => new DoctorSearchDto
                {
                    Id = m.Id.GetHashCode(),
                    Name = m.Nome_completo,
                    Specialty = m.Especialidade,
                    City = m.Codigo_Municipio.Nome,
                    State = m.Codigo_Municipio.Codigo_uf,
                    Hospitals = new List<string> { "Hospital Local" } // Simulado - precisaria de relação real
                })
                .ToListAsync();

            return medicos;
        }
    }
}