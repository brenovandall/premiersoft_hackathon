using Hackathon.Premiersoft.API.Dto;

namespace Hackathon.Premiersoft.API.Engines.Csv
{
    // Modelos de dados
    public class CsvMappedData
    {
        public List<CsvHeader> Headers { get; set; } = new();
        public List<CsvRow> Rows { get; set; } = new();
        public Dictionary<string, int> ColumnMapping { get; set; } = new();
        public CsvMetadata Metadata { get; set; }

        // Métodos de conveniência
        public object GetValue(int rowIndex, string columnName)
        {
            if (rowIndex >= 0 && rowIndex < Rows.Count &&
                Rows[rowIndex].Data.ContainsKey(columnName.ToLowerInvariant()))
            {
                return Rows[rowIndex].Data[columnName.ToLowerInvariant()];
            }
            return null;
        }

        public List<T> GetColumn<T>(string columnName)
        {
            var normalizedName = columnName.ToLowerInvariant().Replace(" ", "_").Replace("-", "_");
            return Rows
                .Where(row => row.Data.ContainsKey(normalizedName))
                .Select(row => (T)Convert.ChangeType(row.Data[normalizedName], typeof(T)))
                .ToList();
        }
    }
}
