using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Messaging.Events;
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
            // todo
        }
    }
}
