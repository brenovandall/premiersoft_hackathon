using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IEntityDto
    {
        Import Import { get; set; }
        public long NumeroLinha { get; set; }
        public string Campo { get; set; }
        public string Valor { get; set; }
    }
}
