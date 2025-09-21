using Hackathon.Premiersoft.API.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models
{
    public class Medicos : Entity<long>
    {
        public string Codigo { get; set; }
        public string Nome_completo { get; set; }
        public string Especialidade { get; set; }

        [Required]
        public long MunicipioId { get; set; }
        [ForeignKey(nameof(MunicipioId))]
        public Municipios Codigo_Municipio { get; set; }
    }
}