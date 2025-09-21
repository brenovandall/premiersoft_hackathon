using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IHospitaisHandler
    {
        public void ProcessarHospitais(IEntityDto dto);
    }
}