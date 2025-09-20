using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Csv;
using System.Globalization;
using System.Text;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class CsvFileReaderEngine : Factory.IFileReaderEngine
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.CsvReaderProvider;
        public void Run(string preSignedUrl)
        {
            try
            {
                var csvContent = File.ReadAllText(preSignedUrl, _options.Encoding);
                var dataCsv = ParseCsvData(csvContent);
            }
            catch (Exception ex)
            {
                var tratamentoErros =  new CsvMappedData
                {
                    Headers = new List<CsvHeader>(),
                    Rows = new List<CsvRow>(),
                    ColumnMapping = new Dictionary<string, int>(),
                    Metadata = new CsvMetadata
                    {
                        ValidationErrors = new List<string> { $"Erro ao ler arquivo: {ex.Message}" },
                        ProcessedAt = DateTime.UtcNow
                    }
                };
            }
        }

        private readonly CsvReaderOptions _options;

        public CsvFileReaderEngine(CsvReaderOptions options = null)
        {
            _options = options ?? new CsvReaderOptions();
        }


        private CsvMappedData ParseCsvData(string csvContent)
        {
            var lines = SplitLines(csvContent);

            if (!lines.Any())
            {
                return CreateEmptyResult("Arquivo CSV vazio");
            }

            var headers = ParseHeaders(lines[0]);
            var rows = ParseRows(lines.Skip(1).ToArray(), headers);

            return new CsvMappedData
            {
                Headers = headers,
                Rows = rows,
                ColumnMapping = CreateColumnMapping(headers),
                Metadata = new CsvMetadata
                {
                    TotalRows = rows.Count,
                    TotalColumns = headers.Count,
                    ProcessedAt = DateTime.UtcNow,
                    ValidationErrors = new List<string>()
                }
            };
        }

        private List<string> SplitLines(string csvContent)
        {
            return csvContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private List<CsvHeader> ParseHeaders(string headerLine)
        {
            var headerValues = SplitCsvLine(headerLine);
            var headers = new List<CsvHeader>();

            for (int i = 0; i < headerValues.Count; i++)
            {
                var header = new CsvHeader
                {
                    Index = i,
                    Name = headerValues[i].Trim(),
                    NormalizedName = NormalizeHeaderName(headerValues[i].Trim()),
                    InferredType = typeof(string) // Inicialmente string, será inferido depois
                };
                headers.Add(header);
            }

            return headers;
        }

        private List<CsvRow> ParseRows(string[] dataLines, List<CsvHeader> headers)
        {
            var rows = new List<CsvRow>();

            for (int lineIndex = 0; lineIndex < dataLines.Length; lineIndex++)
            {
                if (string.IsNullOrWhiteSpace(dataLines[lineIndex]))
                    continue;

                var values = SplitCsvLine(dataLines[lineIndex]);
                var rowData = new Dictionary<string, object>();
                var rawValues = new List<string>();

                for (int i = 0; i < Math.Max(values.Count, headers.Count); i++)
                {
                    var value = i < values.Count ? values[i] : "";
                    var header = i < headers.Count ? headers[i] : null;

                    rawValues.Add(value);

                    if (header != null)
                    {
                        var convertedValue = ConvertValue(value, header);
                        rowData[header.NormalizedName] = convertedValue;

                        // Inferir tipo se ainda é string
                        if (header.InferredType == typeof(string) && !string.IsNullOrEmpty(value))
                        {
                            header.InferredType = InferType(value);
                        }
                    }
                }

                var row = new CsvRow
                {
                    Index = lineIndex,
                    Data = rowData,
                    RawValues = rawValues
                };

                rows.Add(row);
            }

            return rows;
        }

        private List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Aspas duplas escapadas
                        current.Append('"');
                        i++; // Pula a próxima aspa
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == _options.Separator && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result;
        }

        private string NormalizeHeaderName(string headerName)
        {
            return headerName
                .Replace(" ", "_")
                .Replace("-", "_")
                .ToLowerInvariant();
        }

        private Dictionary<string, int> CreateColumnMapping(List<CsvHeader> headers)
        {
            return headers.ToDictionary(h => h.NormalizedName, h => h.Index);
        }

        private Type InferType(string value)
        {
            if (int.TryParse(value, out _))
                return typeof(int);

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                return typeof(double);

            if (DateTime.TryParse(value, out _))
                return typeof(DateTime);

            if (bool.TryParse(value, out _))
                return typeof(bool);

            return typeof(string);
        }

        private object ConvertValue(string value, CsvHeader header)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                return header.InferredType.Name switch
                {
                    "Int32" => int.Parse(value),
                    "Double" => double.Parse(value, CultureInfo.InvariantCulture),
                    "DateTime" => DateTime.Parse(value),
                    "Boolean" => bool.Parse(value),
                    _ => value
                };
            }
            catch
            {
                return value; // Retorna string se conversão falhar
            }
        }

        private CsvMappedData CreateEmptyResult(string error)
        {
            return new CsvMappedData
            {
                Headers = new List<CsvHeader>(),
                Rows = new List<CsvRow>(),
                ColumnMapping = new Dictionary<string, int>(),
                Metadata = new CsvMetadata
                {
                    ValidationErrors = new List<string> { error },
                    ProcessedAt = DateTime.UtcNow
                }
            };
        }
    }
}
