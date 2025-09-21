using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IEstadosHandler
    {
        public void ProcessarEstados(IEntityDto dto);
    }
}