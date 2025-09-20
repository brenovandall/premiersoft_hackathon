using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Models
{
    public class Pacientes : Entity<int>
    {
        public string Codigo { get; set; }
        public string Cpf { get; set; }
        public string Genero { get; set; }
        public string Nome_completo { get; set; }
        public Municipios Codigo_Municipio { get; set; }
        public bool Convenio { get; set; }
        public Cid10 Cid10 { get; set; }

    }
}
