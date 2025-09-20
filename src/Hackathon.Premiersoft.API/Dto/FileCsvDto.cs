using System.Text;

namespace Hackathon.Premiersoft.API.Dto
{
    public class FileCsvDto
    {
    }

    public class CsvHeader
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public Type InferredType { get; set; }
    }

    public class CsvRow
    {
        public int Index { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
        public List<string> RawValues { get; set; } = new();
    }

    public class CsvMetadata
    {
        public int TotalRows { get; set; }
        public int TotalColumns { get; set; }
        public DateTime ProcessedAt { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
    }

    // Opções de configuração
    public class CsvReaderOptions
    {
        public char Separator { get; set; } = ',';
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public bool HasHeaders { get; set; } = true;
        public bool TrimWhitespace { get; set; } = true;
    }
}
