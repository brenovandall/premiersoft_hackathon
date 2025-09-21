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

        public PatientsHospitals()
        {
            Id = Guid.NewGuid();
        }
    }
}
