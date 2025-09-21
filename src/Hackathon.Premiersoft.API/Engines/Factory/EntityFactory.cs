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
        private IMedicosHandler MedicosHandler { get; set; }
        private IMuncipiosHandler MunicipiosHandler { get; set; }

        public EntityFactory(IPacientesHandler pacientesHandler, IMedicosHandler medicosHandler, IMuncipiosHandler muncipiosHandler)
        {
            MedicosHandler = medicosHandler;
            PacientesHandler = pacientesHandler;
            MunicipiosHandler = muncipiosHandler;
        }

        public void CreateEntity(IEntityDto dto)
        {
            switch (dto.Import.DataType)
            {
                case ImportDataTypes.Patient:
                    PacientesHandler.ProcessarPaciente(dto);
                    break;
                case ImportDataTypes.Doctor:
                    MedicosHandler.ProcessarMedicos(dto);
                    break;
                case ImportDataTypes.City:
                    MedicosHandler.ProcessarMedicos(dto);
                    break;
                    //case ImportDataTypes.State:
                    //    return new Estados();
                    //case ImportDataTypes.City:
                    //    return new Municipios();
                    //case ImportDataTypes.Doctor:
                    //    return new Medicos();
                    //    break;
            }
        }
    }
}
