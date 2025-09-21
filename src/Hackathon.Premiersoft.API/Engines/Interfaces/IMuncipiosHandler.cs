using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IMuncipiosHandler
    {
        public void ProcessarMunicipios(IEntityDto dto);
    }
}