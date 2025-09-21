using Hackathon.Premiersoft.API.Dto;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IHl7Process
    {
        void Process(List<FileHl7Dto> hl7Files);
    }
}