using Hackathon.Premiersoft.API.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models
{
    public class DoctorsHospitals : Entity<Guid>
    {
        [Required]
        public Guid DoctorId { get; private set; }

        [ForeignKey(nameof(DoctorId))]
        public Medicos Doctor { get; private set; } = default!;

        [Required]
        public Guid HospitalId { get; private set; }

        [ForeignKey(nameof(HospitalId))]
        public Hospitais Hospital { get; private set; } = default!;

        public DoctorsHospitals(Guid doctorId, Guid hospitalId)
        {
            Id = Guid.NewGuid();
            DoctorId = doctorId;
            HospitalId = hospitalId;
        }
    }
}
