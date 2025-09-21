using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Models
{
    public class Estados : Entity<Guid>
    {
        public int Codigo_uf { get; set; }
        public string Uf { get; set; }
        public string Nome { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Regiao { get; set; }

        public Estados()
        {
            Id = Guid.NewGuid();
        }
    }
}
