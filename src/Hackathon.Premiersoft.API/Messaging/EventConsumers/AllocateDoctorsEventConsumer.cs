using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Utils;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Messaging.EventConsumers
{
    public class AllocateDoctorsEventConsumer : IConsumer<AllocateDoctorsEvent>
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;

        public AllocateDoctorsEventConsumer(IPremiersoftHackathonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<AllocateDoctorsEvent> context)
        {
            var doctors = await _dbContext.Medicos
                .Include(m => m.Codigo_Municipio)
                .Include(m => m.DoctorsHospitals)
                .ToListAsync();

            var hospitals = await _dbContext.Hospitais
                .Include(h => h.Municipio)
                .Include(h => h.DoctorsHospitals)
                .ToListAsync();

            foreach (var doctor in doctors)
            {
                if (doctor.DoctorsHospitals.Count >= 3)
                    continue;

                var comp = hospitals
                    .Where(h => h.Especialidades == doctor.Especialidade)
                    .ToList();

                foreach (var hospital in comp)
                {
                    if (doctor.DoctorsHospitals.Count >= 3)
                        break;

                    if (hospital.DoctorsHospitals.Any(dh => dh.DoctorId == doctor.Id))
                        continue;

                    var dist = GeoUtils.CalculateDistance(
                        (double)doctor.Codigo_Municipio.Latitude,
                        (double)doctor.Codigo_Municipio.Longitude,
                        (double)hospital.Municipio?.Latitude,
                        (double)hospital.Municipio?.Longitude);

                    if (dist <= 30)
                    {
                        await _dbContext.DoctorsHospitals.AddAsync(new DoctorsHospitals(doctor.Id, hospital.Id));
                    }
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
