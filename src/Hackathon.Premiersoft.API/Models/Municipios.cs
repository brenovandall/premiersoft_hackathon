using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Models
{
    public class Municipios : Entity<Guid>
    {
        public string Codigo_ibge { get; set; }
        public string Nome { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool Capital { get; set; }
        public string Codigo_uf { get; set; }
        public int Siafi_id { get; set; }
        public int Ddd { get; set; }
        public string Fuso_horario { get; set; }
        public int Populacao { get; set; }
        public string Erros { get; set; }

        public Municipios()
        {
            Id = Guid.NewGuid();
        }
    }
}
