using Hackathon.Premiersoft.API.Engines.Csv;
using Hackathon.Premiersoft.API.Services;
using System;
using System.Threading.Tasks;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class CsvFileReaderEngine : Factory.IFileReaderEngine
    {
        public string FileReaderProvider => "CsvFileReader";

        public void Run(Guid importId)
        {
            string key = "uploads/municipios/2025-09-20/1758407824810-municipios.csv";

            Task.Run(async () =>
            {
                try
                {
                    var csvReader = new CsvFileReaderProcess();
                    await csvReader.ProcessarArquivoEmBackground(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar arquivo em background: {ex.Message}");
                }
            });
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
            
            });
        }
    }
}
