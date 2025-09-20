using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Csv;
using Hackathon.Premiersoft.API.Engines.Factory;
using System.Globalization;
using System.Text;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class CsvFileReaderEngine : IFileReaderEngine
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.CsvReaderProvider;

        private readonly CsvReaderOptions _options;

        public CsvFileReaderEngine(CsvReaderOptions options = null)
        {
            _options = options ?? new CsvReaderOptions();
        }

        public void Run(string preSignedUrl)
        {
            try
            {
                var csvContent = File.ReadAllText(preSignedUrl, _options.Encoding);
                var dataCsv = ParseCsvData(csvContent);
            }
            catch (Exception ex)
            {
                var tratamentoErros = new CsvMappedData
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

        private CsvMappedData ParseCsvData(string csvContent)
        {
            var lines = SplitLines(csvContent);

            if (!lines.Any())
            {
                return CreateEmptyResult("Arquivo CSV vazio");
            }

            var headers = ParseHeaders(lines[0]);

            if (!headers.Any())
            {
                return CreateEmptyResult("Nenhum cabeçalho válido encontrado.");
            }

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
            var headerValues = SplitCsvLine(headerLine)
                .Where(h => !string.IsNullOrWhiteSpace(h))
                .ToList();

            if (headerValues.Count == 0)
            {
                throw new Exception("Cabeçalhos do CSV estão vazios.");
            }

            var headers = new List<CsvHeader>();

            for (int i = 0; i < headerValues.Count; i++)
            {
                var headerName = headerValues[i].Trim();
                if (string.IsNullOrWhiteSpace(headerName))
                    continue;

                headers.Add(new CsvHeader
                {
                    Index = i,
                    Name = headerName,
                    NormalizedName = NormalizeHeaderName(headerName),
                    InferredType = typeof(string)
                });
            }

            return headers;
        }

        private List<CsvRow> ParseRows(string[] dataLines, List<CsvHeader> headers)
        {
            var rows = new List<CsvRow>();

            for (int lineIndex = 0; lineIndex < dataLines.Length; lineIndex++)
            {
                var line = dataLines[lineIndex];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = SplitCsvLine(line);

                if (values.All(v => string.IsNullOrWhiteSpace(v)))
                    continue;

                var rowData = new Dictionary<string, object>();
                var rawValues = new List<string>();

                for (int i = 0; i < headers.Count; i++)
                {
                    var value = i < values.Count ? values[i] : "";
                    var header = headers[i];

                    rawValues.Add(value);

                    var convertedValue = ConvertValue(value, header);
                    rowData[header.NormalizedName] = convertedValue;

                    if (header.InferredType == typeof(string) && !string.IsNullOrEmpty(value))
                    {
                        header.InferredType = InferType(value);
                    }
                }

                rows.Add(new CsvRow
                {
                    Index = lineIndex,
                    Data = rowData,
                    RawValues = rawValues
                });
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
                        current.Append('"');
                        i++; // Skip next quote
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

        public void Run(long importId)
        {
            throw new NotImplementedException();
        }
    }
}
