using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Engines.Parsers.Hl7;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class Hl7FileReader : IFileReaderEngine
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.Hl7ReaderProvider;
        private IRepository<Import, Guid> Import { get; set; }
        private IHl7Process Hl7Process { get; set; }
        public Hl7FileReader(IRepository<Import, Guid> import, IHl7Process hl7Process)
        {
            Hl7Process = hl7Process;
            Import = import;
        }

        public async Task Run(Guid guid)
        {
            var import = Import.GetById(guid) ?? throw new Exception("Importação não encontrado!");

            if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                throw new Exception("URL do arquivo não encontrado!");

            var xml = File.ReadAllText(import.S3PreSignedUrl);
            var hl7Parser = new Hl7Parser();
            var parsedFiles = await hl7Parser.ParseHl7Async(import);

            Hl7Process.Process(parsedFiles);
        }
    }
}
