using Hackathon.Premiersoft.API.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models
{
    public class PatientsHospitals : Entity<Guid>
    {
        [Required]
        public Guid PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Pacientes Patient { get; set; } = default!;

        [Required]
        public Guid HospitalId { get; set; }

        [ForeignKey(nameof(HospitalId))]
        public Hospitais Hospital { get; set; } = default!;

        /// <summary>
        /// Data e hora da alocação do paciente ao hospital
        /// </summary>
        public DateTime DataAlocacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Distância em quilômetros entre o paciente e o hospital
        /// </summary>
        public double Distancia { get; set; }

        /// <summary>
        /// Status da alocação: Alocado, Transferido, Alta, Cancelado
        /// </summary>
        [MaxLength(50)]
        public string Status { get; set; } = "Alocado";

        /// <summary>
        /// Observações sobre a alocação
        /// </summary>
        [MaxLength(500)]
        public string? Observacoes { get; set; }

        /// <summary>
        /// Prioridade da alocação (1 = Alta, 2 = Média, 3 = Baixa)
        /// </summary>
        public int Prioridade { get; set; } = 2;

        public PatientsHospitals()
        {
            Id = Guid.NewGuid();
        }

        public PatientsHospitals(Guid patientId, Guid hospitalId, DateTime dataAlocacao, double distancia, string status)
        {
            Id = Guid.NewGuid();
            PatientId = patientId;
            HospitalId = hospitalId;
            DataAlocacao = dataAlocacao;
            Distancia = distancia;
            Status = status;
        }
    }
}
