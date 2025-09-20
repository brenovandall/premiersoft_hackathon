using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Csv;
using Hackathon.Premiersoft.API.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.IO;
using System.Text;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class CsvFileReaderEngine : Factory.IFileReaderEngine
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.CsvReaderProvider;

        private readonly CsvReaderOptions _options;

        public CsvFileReaderEngine(CsvReaderOptions options = null)
        {
            _options = options ?? new CsvReaderOptions();
        }

        public void Run(long preSignedUrl)
        {
            string key = "uploads/municipios/2025-09-20/1758407824810-municipios.csv";

            try
            {
                Task.Run(async () =>
                {
                    await ProcessarArquivoEmBackground(key);
                });
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

        private async Task ProcessarArquivoEmBackground(string key)
        {
            try
            {
                var s3Service = new S3Service();
                using var reader = await s3Service.ObterLeitorDoArquivoAsync(key);

                var csvData = await ParseCsvDataAsync(reader);

                // Aqui você pode tratar os dados carregados do CSV como desejar
                Console.WriteLine($"Linhas processadas: {csvData.Rows.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar arquivo em background: {ex.Message}");
            }
        }

        private async Task<CsvMappedData> ParseCsvDataAsync(TextReader reader)
        {
            var headers = new List<CsvHeader>();
            var rows = new List<CsvRow>();

            string headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(headerLine))
            {
                return CreateEmptyResult("Arquivo CSV vazio");
            }

            headers = ParseHeaders(headerLine);

            if (!headers.Any())
            {
                return CreateEmptyResult("Nenhum cabeçalho válido encontrado.");
            }

            var columnMapping = CreateColumnMapping(headers);

            int lineIndex = 0;
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
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
                    Index = lineIndex++,
                    Data = rowData,
                    RawValues = rawValues
                });
            }

            return new CsvMappedData
            {
                Headers = headers,
                Rows = rows,
                ColumnMapping = columnMapping,
                Metadata = new CsvMetadata
                {
                    TotalRows = rows.Count,
                    TotalColumns = headers.Count,
                    ProcessedAt = DateTime.UtcNow,
                    ValidationErrors = new List<string>()
                }
            };
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
        private string NormalizeHeaderName(string headerName)
        {
            return headerName
                .Replace(" ", "_")
                .Replace("-", "_")
                .ToLowerInvariant();
        }
        private List<CsvRow> ParseRows(string[] dataLines, List<CsvHeader> headers)
        {
            // Este método não é mais usado com a versão streaming, pode ser removido se quiser
            throw new NotImplementedException("Use ParseCsvDataAsync para leitura por stream.");
        }
    }
}