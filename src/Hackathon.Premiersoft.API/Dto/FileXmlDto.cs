using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Dto
{
    public class FileXmlDto : IEntityDto
    {
        public long NumeroLinha { get; set; }
        public string Campo { get; set; }
        public string Valor { get; set; }
        public Import Import { get; set; }

        public void Criar(long numeroLinha, string campo, string valor, Import import)
        {
            NumeroLinha = numeroLinha;
            Campo = campo;
            Valor = valor;
            Import = Import;
        }
    }
}
