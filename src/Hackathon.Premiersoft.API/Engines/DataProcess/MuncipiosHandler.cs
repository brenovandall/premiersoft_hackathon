using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class MuncipiosHandler : IMuncipiosHandler
    {
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private IRepository<Municipios, Guid> MunicipiosRepository { get; set; }

        public MuncipiosHandler(IRepository<LineError, Guid> errorLineRepo, IRepository<Municipios, Guid> municipiosRepository)
        {
            MunicipiosRepository = municipiosRepository;
            ErrorLineRepo = errorLineRepo;
        }

        public void ProcessarMunicipios(IEntityDto dto)
        {
            // TODO: Implementar lógica de processamento de municípios
            // Baseado no padrão do MedicosHandler
        }
    }
}