using Azure;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;
using System.Diagnostics;

namespace Hackathon.Premiersoft.API.Engines.Xml
{
    public class XmlProcess : IXmlProcess
    {
        private IRepository<Pacientes, long> PacientesRepository { get; set; }

        public XmlProcess(IRepository<Pacientes, long> pacientesRepo, IRepository<LineError, long> lineErrorRepo, IMunicipiosRepository municipiosRepo)
        {
            PacientesRepository = pacientesRepo;
        }

        public void Process(List<FileXmlDto> xmlFile, Import import)
        {
            xmlFile.ForEach(x => ProcessLine(x, import));
        }

        private void ProcessLine(FileXmlDto dto, Import import)
        {
            var paciente = new Pacientes();
            FeedEntity(dto, paciente, import);
            PacientesRepository.Add(paciente);
        }

        private void FeedEntity(FileXmlDto dto, Pacientes paciente, Import import)
        {
        }


    }
}
