using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IEntityFactory
    {
        IEntity CreateEntity(IEntityDto dto);
    }
}
