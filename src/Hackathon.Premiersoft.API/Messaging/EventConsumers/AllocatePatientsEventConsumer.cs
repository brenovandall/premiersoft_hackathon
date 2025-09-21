using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Utils;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Messaging.EventConsumers
{
    public class AllocatePatientsEventConsumer : IConsumer<AllocatePatientsEvent>
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;

        public AllocatePatientsEventConsumer(IPremiersoftHackathonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<AllocatePatientsEvent> context)
        {
            // Buscar pacientes que ainda não foram alocados ou precisam de realocação
            var patients = await _dbContext.Pacientes
                .Include(p => p.Codigo_Municipio)
                .Include(p => p.Cid10)
                .Include(p => p.PatientsHospitals)
                .ToListAsync();

            // Buscar hospitais com suas especialidades
            var hospitals = await _dbContext.Hospitais
                .Include(h => h.Cidade)
                .Include(h => h.PatientsHospitals)
                .ToListAsync();

            // Buscar mapeamento CID-10 x Especialidade
            var cidEspecialidades = await _dbContext.Cid10Especialidades
                .Include(ce => ce.Cid10)
                .ToListAsync();

            foreach (var patient in patients)
            {
                // Verificar se paciente já está alocado
                if (patient.PatientsHospitals.Any())
                    continue;

                // Encontrar especialidades adequadas para o CID-10 do paciente
                var especialidadesRequeridas = cidEspecialidades
                    .Where(ce => ce.Cid10Id == patient.Cid10Id)
                    .OrderBy(ce => ce.Prioridade)
                    .Select(ce => ce.Especialidade)
                    .ToList();

                if (!especialidadesRequeridas.Any())
                {
                    // Se não há mapeamento específico, usar clínica geral
                    especialidadesRequeridas.Add("Clínica Geral");
                }

                // Encontrar hospitais com especialidades compatíveis
                var hospitaisCompativeis = hospitals
                    .Where(h => especialidadesRequeridas.Any(esp => 
                        h.Especialidades.Contains(esp, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (!hospitaisCompativeis.Any())
                    continue;

                // Calcular distâncias e ordenar por proximidade
                var hospitaisComDistancia = hospitaisCompativeis
                    .Select(h => new
                    {
                        Hospital = h,
                        Distancia = GeoUtils.CalculateDistance(
                            (double)patient.Codigo_Municipio.Latitude,
                            (double)patient.Codigo_Municipio.Longitude,
                            (double)h.Cidade.Latitude,
                            (double)h.Cidade.Longitude)
                    })
                    .OrderBy(hd => hd.Distancia)
                    .ToList();

                // Alocar no hospital mais próximo que tenha capacidade
                foreach (var hospitalDistancia in hospitaisComDistancia)
                {
                    var hospital = hospitalDistancia.Hospital;
                    var distancia = hospitalDistancia.Distancia;

                    // Verificar capacidade do hospital (assumindo limite baseado em leitos)
                    var pacientesAtuais = hospital.PatientsHospitals.Count;
                    var capacidadeMaxima = hospital.Leitos_totais * 0.8; // 80% da capacidade

                    if (pacientesAtuais >= capacidadeMaxima)
                        continue;

                    // Criar alocação
                    await _dbContext.PatientsHospitals.AddAsync(new PatientsHospitals(
                        patient.Id,
                        hospital.Id,
                        DateTime.UtcNow,
                        distancia,
                        "Alocado"
                    ));

                    break; // Paciente alocado, prosseguir para o próximo
                }
            }

            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
