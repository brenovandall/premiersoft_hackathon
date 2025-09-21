using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class EstadosHandler : IEstadosHandler
    {
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private IRepository<Estados, Guid> EstadosRepository { get; set; }

        public EstadosHandler(IRepository<LineError, Guid> errorLineRepo, IRepository<Estados, Guid> estadosRepository)
        {
            EstadosRepository = estadosRepository;
            ErrorLineRepo = errorLineRepo;
        }

        public void ProcessarEstados(IEntityDto dto)
        {
            // TODO: Implementar lógica de processamento de estados
            // Baseado no padrão do MedicosHandler
        }
    }
}