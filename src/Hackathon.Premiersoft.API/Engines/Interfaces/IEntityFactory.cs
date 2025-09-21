using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IEntityFactory
    {
        void CreateEntity(IEntityDto dto);
    }
}
