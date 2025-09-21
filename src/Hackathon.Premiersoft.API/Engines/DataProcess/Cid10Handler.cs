using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class Cid10Handler : ICid10Handler
    {
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private IRepository<Cid10, Guid> Cid10Repository { get; set; }

        public Cid10Handler(IRepository<LineError, Guid> errorLineRepo, IRepository<Cid10, Guid> cid10Repository)
        {
            Cid10Repository = cid10Repository;
            ErrorLineRepo = errorLineRepo;
        }

        public void ProcessarCid10(IEntityDto dto)
        {
            // TODO: Implementar lógica de processamento de CID-10
            // Baseado no padrão do MedicosHandler
        }
    }
}