using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Models
{
    public class Medicos : Entity<int>
    {
        public string Codigo { get; set; }
        public string Nome_completo { get; set; }
        public string Especialidade { get; set; }
    }
}