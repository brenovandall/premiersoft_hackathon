using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IMedicosHandler
    {
        public IEntity ProcessarMedidos(IEntityDto dto);
    }
}
