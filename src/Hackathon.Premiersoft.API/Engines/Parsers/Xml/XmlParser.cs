using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services;
using System.Xml;

namespace Hackathon.Premiersoft.API.Engines.Parsers.Xml
{
    public class XmlParser : IXmlParser
    {
        private IEntityFactory EntityFactory { get; set; }

        public XmlParser(IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        public async Task ParseXmlAsync(Import import)
        {
            var s3Service = new S3Service();

            using var readerS3 = await s3Service.ObterLeitorDoArquivoAsync(import.S3PreSignedUrl);

            var settings = new XmlReaderSettings
            {
                Async = true,
                IgnoreWhitespace = true
            };

            using var xmlReader = XmlReader.Create(readerS3, settings);
            while (await xmlReader.ReadAsync())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    string tagName = xmlReader.Name;

                    if (await xmlReader.ReadAsync() && xmlReader.NodeType == XmlNodeType.Text)
                    {
                        string value = xmlReader.Value;

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            int lineNumber = 0;
                            if (xmlReader is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
                            {
                                lineNumber = lineInfo.LineNumber;
                            }

                            EntityFactory.CreateEntity(
                               new FileXmlDto
                               {
                                   Import = import,
                                   NumeroLinha = lineNumber,
                                   Campo = tagName,
                                   Valor = value
                               });

                            Console.WriteLine($"Line {lineNumber} - {tagName}: {value}");
                        }
                    }
                }
            }
        }
    }
}
