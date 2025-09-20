
using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IPacientesHandler
    {
        public Pacientes ProcessarPaciente(IEntityDto dto);
    }

}
