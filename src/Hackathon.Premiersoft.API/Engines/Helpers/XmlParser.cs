using Hackathon.Premiersoft.API.Dto;
using System.Xml;

namespace Hackathon.Premiersoft.API.Engines.Helpers
{
    public class XmlParser
    {
        private List<FileXmlDto> FileXmlDtos { get; set; } = new List<FileXmlDto>();
        private static Stream GenerateStreamFromString(string xml)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public List<FileXmlDto> ParseXml(string xml)
        {
            using Stream stream = GenerateStreamFromString(xml);

            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = true,
                IgnoreWhitespace = true
            };

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string tagName = reader.Name;

                        if (reader.Read() && reader.NodeType == XmlNodeType.Text)
                        {
                            string value = reader.Value;

                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                int lineNumber = 0;
                                if (reader is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
                                {
                                    lineNumber = lineInfo.LineNumber;
                                }

                             //   FileXmlDtos.Add(new FileXmlDto().Criar(lineNumber, tagName, value));

                                Console.WriteLine($"Line {lineNumber} - {tagName}: {value}");
                            }
                        }
                    }
                }
            }

            return FileXmlDtos;
        }
    }
}
