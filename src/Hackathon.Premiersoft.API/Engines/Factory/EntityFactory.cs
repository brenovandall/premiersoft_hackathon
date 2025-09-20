using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.DataProcess;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Models.Enums;
using Microsoft.EntityFrameworkCore.Internal;

namespace Hackathon.Premiersoft.API.Engines.Factory
{
    public class EntityFactory : IEntityFactory
    {
        private IPacientesHandler PacientesHandler { get; set; }
        private IPacientesHandler MedicosHandler { get; set; }
        private IPacientesHandler MunicipiosHandler { get; set; }

        public EntityFactory(IPacientesHandler pacientesHandler )
        {
            PacientesHandler = pacientesHandler;
        }

        public IEntity CreateEntity(IEntityDto dto)
        {
            switch (dto.Import.DataType)
            {
                case ImportDataTypes.Patient:
                    return PacientesHandler.ProcessarPaciente(dto);
                case ImportDataTypes.Hospital:
                    return new Hospitais();
                case ImportDataTypes.State:
                    return new Estados();
                case ImportDataTypes.City:
                    return new Municipios();
                case ImportDataTypes.Doctor:
                    return new Medicos();
                    break;
            }
            return null;
        }
    }
}
