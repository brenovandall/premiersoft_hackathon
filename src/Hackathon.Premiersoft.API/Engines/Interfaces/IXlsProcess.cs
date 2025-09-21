using Hackathon.Premiersoft.API.Dto;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IXlsProcess
    {
        public void Process(List<FileXlsDto> xmlFile);
    }
}
