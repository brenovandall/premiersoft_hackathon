using DocumentFormat.OpenXml.InkML;
using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.DataProcess.Helpers;
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
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private readonly IRepository<Medicos, Guid> _medicosRepository;
        private IPremiersoftHackathonDbContext Context { get; set; }
        public MedicosHandler(IPremiersoftHackathonDbContext context, IRepository<LineError, Guid> errorLineRepo, IMunicipiosRepository municipiosRepository, IRepository<Medicos, Guid> medicosRepository)
        {
            Context = context;
            _medicosRepository = medicosRepository;
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

                if (DataHelper.CheckCampo(tag, nameof(medico.Especialidade)))
                    medico.Especialidade = value;

                if (DataHelper.CheckCampo(tag, nameof(medico.Codigo)))
                    medico.Codigo = value;

                if (DataHelper.CheckCampo(tag, nameof(medico.Codigo_Municipio)))
                {
                    var municipio = MunicipiosRepository.GetByIbgeCode(value);

                    if (municipio == null)
                        throw new Exception($"Município não encontrado para código IBGE {value}");

                    medico.MunicipioId = municipio.Id;
                }

                var set = Context.Set<Medicos>();
                set.Add(medico);
                Context.SaveChanges();
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

                var set = Context.Set<LineError>();
                set.Add(error);
                Context.SaveChanges();
            }
        }
    }
}
