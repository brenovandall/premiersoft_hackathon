using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IXmlProcess
    {
        public void Process(FileXmlDto xmlFile);
    }
}
