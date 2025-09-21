using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace Hackathon.Premiersoft.API.Engines.Csv
{
    public class CsvFileReaderProcess: ICsvFileReaderProcess
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.CsvReaderProvider;
  
        private IRepository<Municipios, Guid> MunicipiosRepository { get; set; }

 
        public CsvFileReaderProcess( IRepository<Municipios, Guid> municipiosRepository)
        {
            MunicipiosRepository = municipiosRepository;
        }


        public async Task ProcessarArquivoEmBackground(string key)
        {
            try
            {
                var s3Service = new S3Service();
                using var reader = await s3Service.ObterLeitorDoArquivoAsync(key);

                var csvData = await ParseCsvDataAsync(reader);

                if (csvData != null && csvData.Rows.Any())
                {
                    await ImportarMunicipiosAsync(csvData);
                }

                Console.WriteLine($"Linhas lidas do arquivo: {csvData.Rows.Count}");
            }
            catch (Exception ex)
            {
                // Este bloco agora captura erros tanto da leitura do arquivo quanto da importação
                Console.WriteLine($"Erro no processamento em background: {ex.Message}");
            }
        }


        public async Task ImportarMunicipiosAsync(CsvMappedData csvData)
        {
            // 1. DEFINIR O "DE-PARA" (Mapeamento)
            // Key: Nome da coluna no arquivo (já normalizado pelo seu leitor)
            // Value: Nome da propriedade na sua entidade `Municipios`
            var mapeamentoMunicipios = new Dictionary<string, string>
            {
                { "codigo_ibge", nameof(Municipios.Codigo_ibge) },
                { "nome", nameof(Municipios.Nome) },
                { "latitude", nameof(Municipios.Latitude) },
                { "longitude", nameof(Municipios.Longitude) },
                { "capital", nameof(Municipios.Capital) },
                { "codigo_uf", nameof(Municipios.Codigo_uf) },
                { "siafi_id", nameof(Municipios.Siafi_id) },
                { "ddd", nameof(Municipios.Ddd) },
                { "fuso_horario", nameof(Municipios.Fuso_horario) }
            };

            var engine = new GenericDataInsertEngine<Municipios, Guid>(MunicipiosRepository);


            var result = await engine.ProcessAndInsertAsync(csvData.Rows, mapeamentoMunicipios);

            // 4. ANALISAR E LOGAR O RESULTADO
            Console.WriteLine($"[DataImportService] Processamento de Municípios concluído.");
            Console.WriteLine($"--> Sucesso: {result.SuccessCount} linhas inseridas.");
            Console.WriteLine($"--> Erros: {result.ErrorCount} linhas com erro.");

            if (result.HasErrors)
            {
                Console.WriteLine("--> Detalhes dos erros:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - Linha {error.RowIndex}, Coluna '{error.ColumnName}': {error.ErrorMessage} (Valor lido: '{error.RawValue}')");
                    // Aqui você pode salvar esses erros em uma tabela de log, como a sua `LineError`
                }
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
                else if (c == ',' && !inQuotes)
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
    }
}
