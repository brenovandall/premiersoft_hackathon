using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Helpers;
using Hackathon.Premiersoft.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class XmlFileReader : IFileReaderEngine
    {
        private PremiersoftHackathonDbContext _dbContext { get; set; }
        public XmlFileReader(PremiersoftHackathonDbContext  dbContext)
        {
            _dbContext = dbContext;
        }

        public string FileReaderProvider => Extensions.FileReaderProvider.XmlReaderProvider;

        public void Run(long importId)
        {
            //var import = _dbContext.Imports.FirstOrDefault(x => x.Id == importId);

            //    var xml = File.ReadAllText(preSignedUrl);
            //var xmlParser = new XmlParser();
            //xmlParser.ParseXml(xml);
        }
    }
}
