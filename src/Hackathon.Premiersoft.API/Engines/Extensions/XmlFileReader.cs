using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Helpers;
using System.Xml;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class XmlFileReader : IFileReaderEngine
    {
        public string FileReaderProvider => ".xml";

        public void Run(string preSignedUrl)
        {
            var xml = File.ReadAllText(preSignedUrl);
            var xmlParser = new XmlParser();
            xmlParser.ParseXml(xml);
        }
    }
}
