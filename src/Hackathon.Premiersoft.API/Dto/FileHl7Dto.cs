using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Dto
{
    public class FileHl7Dto : IEntityDto
    {
        public Import Import { get; set; }
        public long NumeroLinha { get; set; }
        public string Campo { get; set ; }
        public string Valor { get; set; }
    }
}
