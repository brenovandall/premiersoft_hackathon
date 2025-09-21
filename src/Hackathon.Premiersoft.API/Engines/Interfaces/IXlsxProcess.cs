using Hackathon.Premiersoft.API.Dto;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IXlsxProcess
    {
        public void Process(List<FileXlsxDto> xmlFile);
    }
}
