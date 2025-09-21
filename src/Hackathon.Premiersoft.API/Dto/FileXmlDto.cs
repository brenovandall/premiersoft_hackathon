using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using System.Globalization;

namespace Hackathon.Premiersoft.API.Dto
{
    public class FileXmlDto : IEntityDto
    {
        public long NumeroLinha { get; set; }
        public string Campo { get; set; }
        public string Valor { get; set; }
        public Import Import { get; set; }

        public void Criar(long numeroLinha, string campo, string valor, Import import)
        {
            NumeroLinha = numeroLinha;
            Campo = campo;
            Valor = valor;
            Import = Import;
        }
    }

    // Estrutura de dados XML mapeada similar ao CSV
    public class XmlMappedData
    {
        public List<XmlElementInfo> Elements { get; set; } = new();
        public List<XmlRecord> Records { get; set; } = new();
        public Dictionary<string, int> ElementMapping { get; set; } = new();
        public XmlMetadata Metadata { get; set; }

        // Métodos de conveniência
        public object GetValue(int recordIndex, string elementName)
        {
            if (recordIndex >= 0 && recordIndex < Records.Count &&
                Records[recordIndex].Data.ContainsKey(elementName.ToLowerInvariant()))
            {
                return Records[recordIndex].Data[elementName.ToLowerInvariant()];
            }
            return null;
        }

        public List<T> GetElement<T>(string elementName)
        {
            var normalizedName = elementName.ToLowerInvariant().Replace(" ", "_").Replace("-", "_");
            return Records
                .Where(record => record.Data.ContainsKey(normalizedName))
                .Select(record => (T)Convert.ChangeType(record.Data[normalizedName], typeof(T)))
                .ToList();
        }
    }

    public class XmlElementInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public Type InferredType { get; set; }
        public string XPath { get; set; }
    }

    public class XmlRecord
    {
        public int Index { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
        public List<string> RawValues { get; set; } = new();
        public string XmlPath { get; set; }
    }

    public class XmlMetadata
    {
        public int TotalRecords { get; set; }
        public int TotalElements { get; set; }
        public DateTime ProcessedAt { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        public string RootElement { get; set; }
        public List<string> Namespaces { get; set; } = new();
    }
}
