using Hackathon.Premiersoft.API.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models
{
    /// <summary>
    /// Tabela de relacionamento entre CID-10 e especialidades médicas
    /// Define qual especialidade é mais adequada para tratar cada diagnóstico
    /// </summary>
    public class Cid10Especialidade : Entity<Guid>
    {
        [Required]
        public Guid Cid10Id { get; set; }

        [ForeignKey(nameof(Cid10Id))]
        public Cid10 Cid10 { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Especialidade { get; set; } = default!;

        /// <summary>
        /// Prioridade da especialidade para este CID (1 = mais prioritária)
        /// Permite múltiplas especialidades para o mesmo CID
        /// </summary>
        public int Prioridade { get; set; } = 1;

        /// <summary>
        /// Indica se esta é a especialidade primária para este CID
        /// </summary>
        public bool EspecialidadePrimaria { get; set; } = true;

        /// <summary>
        /// Observações sobre o tratamento ou especialidade
        /// </summary>
        [MaxLength(500)]
        public string? Observacoes { get; set; }

        public Cid10Especialidade()
        {
            Id = Guid.NewGuid();
        }
    }
}