using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Helpers;
using Hackathon.Premiersoft.API.Engines.Xml;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class XmlFileReader : IFileReaderEngine
    {
        private IRepository<Import, long> Import { get; set; }
        private IXmlProcess XmlProcessEngine { get; set; }
        public string FileReaderProvider => Extensions.FileReaderProvider.XmlReaderProvider;
        public XmlFileReader(IXmlProcess xmlProcess, IRepository<Import, long> importsRepo)
        {
            XmlProcessEngine = xmlProcess;
            Import = importsRepo;
        }

        public void Run(long importId)
        {
            var import = Import.GetById(importId) ?? throw new Exception("Importação não encontrado!");

            if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                throw new Exception("URL do arquivo não encontrado!");
            //var import = _dbContext.Imports.FirstOrDefault(x => x.Id == importId);

            var xml = File.ReadAllText(import.S3PreSignedUrl);
            var xmlParser = new XmlParser();
            var parsedFiles = xmlParser.ParseXml(xml);
            XmlProcessEngine.Process(parsedFiles, import);
            //    var xml = File.ReadAllText(preSignedUrl);
            //var xmlParser = new XmlParser();
            //xmlParser.ParseXml(xml);
        }
    }
}
