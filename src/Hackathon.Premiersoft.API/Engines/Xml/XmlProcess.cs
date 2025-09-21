using Azure;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;
using System.Diagnostics;

namespace Hackathon.Premiersoft.API.Engines.Xml
{
    public class XmlProcess : IXmlProcess
    {
        private IEntityFactory EntityFactory { get; set; }
        public XmlProcess(IEntityFactory entityFactory) => EntityFactory = entityFactory;
        public void Process(List<FileXmlDto> xmlFile) => xmlFile.ForEach(x => ProcessLine(x));
        private void ProcessLine(FileXmlDto dto) => EntityFactory.CreateEntity(dto);
    }
}
