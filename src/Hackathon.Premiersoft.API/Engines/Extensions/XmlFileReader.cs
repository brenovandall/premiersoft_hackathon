using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Services;
using System.Diagnostics;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class XmlFileReader : IFileReaderEngine
    {
        private IRepository<Import, Guid> Import { get; set; }
        private IXmlParser XmlParser { get; set; }
        public string FileReaderProvider => Extensions.FileReaderProvider.XmlReaderProvider;
        public XmlFileReader(IRepository<Import, Guid> importsRepo, IXmlParser xmlParser)
        {
            XmlParser = xmlParser;
            Import = importsRepo;
        }

        public async Task Run(Guid importId)
        {
            var contador = new Stopwatch();
            contador.Start();

            var import = Import.GetById(importId) ?? throw new Exception("Importação não encontrado!");

            if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                throw new Exception("URL do arquivo não encontrado!");

            await XmlParser.ParseXmlAsync(import);
            contador.Stop();

            Console.WriteLine($"Total segundos: {contador.Elapsed.Seconds.ToString()}");
        }
    }
}
