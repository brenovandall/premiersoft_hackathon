using Hackathon.Premiersoft.API.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models
{
    public class Pacientes : Entity<long>
    {
        [Required]
        public long Codigo_MunicipioId { get; set; }

        [ForeignKey(nameof(Codigo_MunicipioId))]
        public Municipios Codigo_Municipio { get; private set; } = default!;

        public string Codigo { get; set; }
        public string Cpf { get; set; }
        public string Genero { get; set; }
        public string Nome_completo { get; set; }
        public bool Convenio { get; set; }
        [Required]
        public long Cid10Id { get; set; }

        [ForeignKey(nameof(Cid10Id))]
        public Cid10 Cid10 { get; set; }
    }
}
