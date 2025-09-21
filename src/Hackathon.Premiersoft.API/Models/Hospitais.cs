using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Models
{
    public class Hospitais : Entity<long>
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Bairro { get; set; }
        public Municipios Cidade { get; set; }
        public long Leitos_totais { get; set; }
    }
}
