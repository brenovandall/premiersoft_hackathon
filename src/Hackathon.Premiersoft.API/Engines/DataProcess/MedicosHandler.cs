using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Repository.Municipios;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class MedicosHandler : IMedicosHandler
    {
        private IMunicipiosRepository MunicipiosRepository { get; set; }
        private IRepository<LineError, long> ErrorLineRepo { get; set; }
        private IRepository<Medicos, long> MedicosRepository { get; set; }

        public MedicosHandler(IRepository<LineError, long> errorLineRepo, IMunicipiosRepository municipiosRepository, IRepository<Medicos, long> medicosRepository)
        {
            MedicosRepository = medicosRepository;
            MunicipiosRepository = municipiosRepository;
            ErrorLineRepo = errorLineRepo;
        }

        public void ProcessarMedicos(IEntityDto dto)
        {
            try
            {
                var medico = new Medicos();

                var tag = dto.Campo;
                var value = dto.Valor;

                if (CheckTag(tag, nameof(medico.Especialidade)))
                    medico.Especialidade = value;

                if (CheckTag(tag, nameof(medico.Codigo)))
                    medico.Codigo = value;

                if (CheckTag(tag, nameof(medico.Codigo_Municipio)))
                {
                    var municipio = MunicipiosRepository.GetByIbgeCode(value);

                    if (municipio == null)
                        throw new Exception($"Município não encontrado para código IBGE {value}");

                    medico.MunicipioId = municipio.Id;
                }

                MedicosRepository.Add(medico);
            }
            catch (Exception ex)
            {

                var error = new LineError(
                    importId: dto.Import.Id,
                    import: dto.Import,
                    line: dto.NumeroLinha,
                    field: dto.Campo,
                    error: ex.Message,
                    value: dto.Valor
                );

                ErrorLineRepo.Add(error);
            }
        }

        private bool CheckTag(string tag, string campo)
        {
            return tag == campo;
        }
    }
}
