using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Xml
{
    public interface IXmlProcess
    {
        public void Process(List<FileXmlDto> xmlFile, Import import);
    }
}
