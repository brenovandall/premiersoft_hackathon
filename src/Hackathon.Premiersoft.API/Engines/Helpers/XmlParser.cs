using System.Xml;

namespace Hackathon.Premiersoft.API.Engines.Helpers
{
    public class XmlParser
    {
        public IList<string> Tags { get; private set; } = new List<string>();
        public IList<string> Values { get; private set; } = new List<string>();
        public IDictionary<string, string> TagValuePair { get; private set; } = new Dictionary<string, string>();

        private static Stream GenerateStreamFromString(string xml)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public void ParseXml(string xml)
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
                                Tags.Add(tagName);
                                Values.Add(value);
                                TagValuePair[tagName] = value;

                                Console.WriteLine($"{tagName}: {value}");
                            }
                        }
                    }
                }
            }
        }
    }
}
