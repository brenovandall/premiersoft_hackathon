using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Models
{
    public class Hospitais : Entity<Guid>
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Bairro { get; set; }
        public string Especialidades { get; set; }
        public Municipios Cidade { get; set; }
        public long Leitos_totais { get; set; }

        public ICollection<DoctorsHospitals> DoctorsHospitals { get; private set; } = [];
        public ICollection<PatientsHospitals> PatientsHospitals { get; private set; } = [];

        public Hospitais()
        {
            Id = Guid.NewGuid();
        }
    }
}
