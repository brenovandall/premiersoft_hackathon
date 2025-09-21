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
                    Erros = m.Erros
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
                    Erros = m.Erros
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
    }
}