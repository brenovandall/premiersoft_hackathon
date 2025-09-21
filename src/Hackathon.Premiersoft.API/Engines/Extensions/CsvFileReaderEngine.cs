using Hackathon.Premiersoft.API.Engines.Csv;
using Hackathon.Premiersoft.API.Services;
using System;
using System.Threading.Tasks;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class CsvFileReaderEngine : Factory.IFileReaderEngine
    {
        public string FileReaderProvider => "CsvFileReader";

        public void Run(long importId)
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
    }
}
