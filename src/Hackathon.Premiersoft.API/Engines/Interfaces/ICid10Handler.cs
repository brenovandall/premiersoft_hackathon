using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface ICid10Handler
    {
        public void ProcessarCid10(IEntityDto dto);
    }
}