using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.Models.Enums;
using MassTransit;

namespace Hackathon.Premiersoft.API.Messaging.EventConsumers
{
    public class ImportFileEventConsumer : IConsumer<ImportFileEvent>
    {
        private readonly IFileReaderEngineFactory _fileReaderFactory;

        public ImportFileEventConsumer(IFileReaderEngineFactory fileReaderFactory)
        {
            _fileReaderFactory = fileReaderFactory;
        }

        public async Task Consume(ConsumeContext<ImportFileEvent> context)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Head, context.Message.PreSignedUrl);
            var response = await client.SendAsync(request);

            var isFileReady = response.IsSuccessStatusCode;

            if (isFileReady)
            {
                var fileExtension = ((ImportFileFormat)context.Message.FileFormat).ToString();

                var factory = GetFactory(fileExtension);
                factory.Run(context.Message.ImportId);
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
