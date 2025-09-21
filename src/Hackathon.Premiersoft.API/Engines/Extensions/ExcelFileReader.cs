using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class ExcelFileReader : IFileReaderEngine
    {
        private readonly IRepository<Import, Guid> _importRepository;
        private readonly IXlsxParser _xlsxParser;
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly RecordProcessingService _recordProcessingService;

        public string FileReaderProvider => Extensions.FileReaderProvider.ExcelReaderProvider;

        public ExcelFileReader(
            IRepository<Import, Guid> importsRepo, 
            IXlsxParser xlsxParser,
            IPremiersoftHackathonDbContext dbContext,
            RecordProcessingService recordProcessingService)
        {
            _xlsxParser = xlsxParser;
            _importRepository = importsRepo;
            _dbContext = dbContext;
            _recordProcessingService = recordProcessingService;
        }

        public async Task Run(Guid importId)
        {
            var import = await _dbContext.Imports.FirstOrDefaultAsync(i => i.Id == importId);
            if (import == null)
            {
                throw new Exception($"Import com ID {importId} não encontrado");
            }

            if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                throw new Exception("URL do arquivo não encontrado!");

            // Processa o Excel usando o parser existente mas com nova lógica de salvamento
            await ProcessExcelWithRecordByRecord(import);
        }

        private async Task ProcessExcelWithRecordByRecord(Import import)
        {
            try
            {
                // Usa o parser existente mas modifica para processar registro por registro
                await _xlsxParser.ParseXlsxAsync(import);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar Excel para import {import.Id}: {ex.Message}");
                throw;
            }
        }
    }
}
