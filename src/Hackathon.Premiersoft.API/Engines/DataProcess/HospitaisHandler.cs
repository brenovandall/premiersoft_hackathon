using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class HospitaisHandler : IHospitaisHandler
    {
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private IRepository<Hospitais, Guid> HospitaisRepository { get; set; }

        public HospitaisHandler(IRepository<LineError, Guid> errorLineRepo, IRepository<Hospitais, Guid> hospitaisRepository)
        {
            HospitaisRepository = hospitaisRepository;
            ErrorLineRepo = errorLineRepo;
        }

        public void ProcessarHospitais(IEntityDto dto)
        {
            // TODO: Implementar lógica de processamento de hospitais
            // Baseado no padrão do MedicosHandler
        }
    }
}