using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.Models.Enums;
using MassTransit;

namespace Hackathon.Premiersoft.API.Messaging.EventConsumers
{
    public class ImportFileEventConsumer : IConsumer<ImportFileEvent>
    {
        private readonly IFileReaderEngineFactory _fileReaderFactory;
        private readonly IPremiersoftHackathonDbContext _premiersoftHackathonDbContext;

        public ImportFileEventConsumer(IFileReaderEngineFactory fileReaderFactory, IPremiersoftHackathonDbContext premiersoftHackathonDbContext)
        {
            _fileReaderFactory = fileReaderFactory;
            _premiersoftHackathonDbContext = premiersoftHackathonDbContext;
        }

        public async Task Consume(ConsumeContext<ImportFileEvent> context)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Head, context.Message.PreSignedUrl);
            var response = await client.SendAsync(request);

            var isFileReady = response.IsSuccessStatusCode;

            if (isFileReady)
            {
                var importId = context.Message.ImportId;

                var importEntity = _premiersoftHackathonDbContext
                    .Imports.FirstOrDefault(i => i.Id == importId);

                if (importEntity == null)
                    return;

                importEntity.UpdateStatus(ImportStatus.Processing);

                var fileExtension = ((ImportFileFormat)context.Message.FileFormat).ToString();

                var factory = GetFactory(fileExtension);
                factory.Run(importId);

                var hasErrors = _premiersoftHackathonDbContext
                    .LineErrors.Any(l => l.ImportId == importId);

                if (hasErrors)
                {
                    importEntity.UpdateStatus(ImportStatus.DoneWithWarnings);
                }
                else
                {
                    importEntity.UpdateStatus(ImportStatus.Done);
                }
            }
            else
            {
                await context.SchedulePublish(DateTime.Now.AddSeconds(30), context.Message);
            }
        }

        private IFileReaderEngine GetFactory(string extension)
        {
            var factory = _fileReaderFactory.CreateFactory(extension);

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory;
        }
    }
}
