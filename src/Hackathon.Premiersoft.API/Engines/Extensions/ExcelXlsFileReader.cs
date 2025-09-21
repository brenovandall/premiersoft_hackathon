using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class ExcelXlsFileReader : IFileReaderEngine
    {
        private IRepository<Import, Guid> Import { get; set; }
        private IXlsParser XlsParser { get; set; }
        public string FileReaderProvider => Extensions.FileReaderProvider.XlsReaderProvider;
        public ExcelXlsFileReader(IRepository<Import, Guid> importsRepo, IXlsParser xlsParser)
        {
            XlsParser = xlsParser;
            Import = importsRepo;
        }

        public async Task Run(Guid importId)
        {
            var import = Import.GetById(importId) ?? throw new Exception("Importação não encontrado!");

            if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                throw new Exception("URL do arquivo não encontrado!");

            await XlsParser.ParseXlsAsync(import);
        }
    }
}
