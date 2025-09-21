using Hackathon.Premiersoft.API.Engines.DataProcess.Helpers;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class PacientesHandler : IPacientesHandler
    {
        private IMunicipiosRepository MunicipiosRepository { get; set; }
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private IRepository<Pacientes, Guid> PacientesRepository { get; set; } 

        public PacientesHandler(IMunicipiosRepository municipiosRepository, IRepository<LineError, Guid> errorLineRepo)
        {
            MunicipiosRepository = municipiosRepository;
        }

        public void ProcessarPaciente(IEntityDto dto)
        {
            try
            {
                var paciente = new Pacientes();

                var tag = dto.Campo;
                var value = dto.Valor;

                if (DataHelper.CheckCampo(tag, nameof(paciente.Cpf)))
                    paciente.Cpf = value;

                if (DataHelper.CheckCampo(tag, nameof(paciente.Genero)))
                    paciente.Genero = value;

                if (DataHelper.CheckCampo(tag, nameof(paciente.Nome_completo)))
                    paciente.Nome_completo = value;

                if (DataHelper.CheckCampo(tag, nameof(paciente.Convenio)))
                    paciente.Convenio = bool.TryParse(value, out var convenio)
                        ? convenio
                        : throw new Exception("Valor inválido para convenio");

                if (DataHelper.CheckCampo(tag, nameof(paciente.Codigo_Municipio)))
                {
                    var municipio = MunicipiosRepository.GetByIbgeCode(value);

                    if (municipio == null)
                        throw new Exception($"Município não encontrado para código IBGE {value}");

                    paciente.Codigo_MunicipioId = municipio.Id;
                }

                if (DataHelper.CheckCampo(tag, nameof(paciente.Cid10)))
                {
                    paciente.Cid10Id = Guid.TryParse(value, out var cid10Id)
                        ? cid10Id
                        : throw new Exception("Valor inválido para Cid10Id");
                }

                PacientesRepository.Add(paciente);
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
    }
}
