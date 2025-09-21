using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.Models.Enums;
using Hackathon.Premiersoft.API.Services;
using Hackathon.Premiersoft.API.SharedKernel;
using MassTransit;

namespace Hackathon.Premiersoft.API.Messaging.EventConsumers
{
    public class ImportFileEventConsumer : IConsumer<ImportFileEvent>
    {
        private readonly IFileReaderEngineFactory _fileReaderFactory;
        private readonly IDomainEventsDispatcher _domainEventsDispatcher;
        private readonly IPremiersoftHackathonDbContext _premiersoftHackathonDbContext;

        public ImportFileEventConsumer(
            IFileReaderEngineFactory fileReaderFactory,
            IDomainEventsDispatcher domainEventsDispatcher,
            IPremiersoftHackathonDbContext premiersoftHackathonDbContext)
        {
            _fileReaderFactory = fileReaderFactory;
            _domainEventsDispatcher = domainEventsDispatcher;
            _premiersoftHackathonDbContext = premiersoftHackathonDbContext;
        }

        public async Task Consume(ConsumeContext<ImportFileEvent> context)
        {
            if (await IsBucketReady(context.Message.PreSignedUrl))
            {
                var importId = context.Message.ImportId;

                var importEntity = _premiersoftHackathonDbContext
                    .Imports.FirstOrDefault(i => i.Id == importId);

                if (importEntity == null)
                    return;

                importEntity.UpdateStatus(ImportStatus.Processing);
                _premiersoftHackathonDbContext.SaveChanges();

                var fileExtension = ((ImportFileFormat)context.Message.FileFormat).ToString();

                var factory = GetFactory(fileExtension);
                await factory.Run(importId);

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
                _premiersoftHackathonDbContext.SaveChanges();

                if (importEntity.DataType == ImportDataTypes.Doctor)
                {
                    await DispatchDoctors();
                }
                else if (importEntity.DataType == ImportDataTypes.Patient)
                {
                    await DispatchPatients();
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

        private async Task DispatchDoctors()
        {
            var events = new List<AllocateDoctorsEvent>() { new(Guid.NewGuid()) };
            await _domainEventsDispatcher.DispatchAsync(events);
        }

        private async Task DispatchPatients()
        {
            var events = new List<AllocatePatientsEvent>() { new(Guid.NewGuid()) };
            await _domainEventsDispatcher.DispatchAsync(events);
        }

        private async Task<bool> IsBucketReady(string bucketKey)
        {
            var s3Client = S3Service.GetS3Client();
            var headResponse = await s3Client.GetObjectMetadataAsync("premiersoft-hackathon-uploads", bucketKey);
            return headResponse.ContentLength > 0;
        }
    }
}
