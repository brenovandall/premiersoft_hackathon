using Hackathon.Premiersoft.API.Dto;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IHl7Process
    {
        public void Process(List<FileHl7Dto> xmlFile);
    }
}