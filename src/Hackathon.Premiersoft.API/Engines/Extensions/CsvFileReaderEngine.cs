using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Csv;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models; // Add this using for Municipios
using Hackathon.Premiersoft.API.Repository; // Add this using for IRepository
using Hackathon.Premiersoft.API.Repository.Municipios;
using System;
using System.Threading.Tasks;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class CsvFileReaderEngine : IFileReaderEngine
    {
        private ICsvFileReaderProcess CsvFileReaderProcess { get; set; }
        public string FileReaderProvider => Extensions.FileReaderProvider.CsvReaderProvider;

        private IRepository<Import, Guid> Import { get; set; }

        public CsvFileReaderEngine(ICsvFileReaderProcess cvFileReaderProcess, IRepository<Import, Guid> import)
        {

            CsvFileReaderProcess = cvFileReaderProcess;
            Import = import;
        }
        public void Run(Guid importId)
        {
            string key = "uploads/municipios/2025-09-20/1758407824810-municipios.csv";

            Task.Run(async () =>
            {
                try
                {
                    var import = Import.GetById(importId) ?? throw new Exception("Importação não encontrado!");

                    if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                        throw new Exception("URL do arquivo não encontrado!");
                    // 3. Pass the required repository instance to the constructor
                    await CsvFileReaderProcess.ProcessarArquivoEmBackground(import.S3PreSignedUrl);

                     
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar arquivo em background: {ex.Message}");
                }
            });
        }
    }
}