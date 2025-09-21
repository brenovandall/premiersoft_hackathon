using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Engines.Parsers.Hl7;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class Hl7FileReader : IFileReaderEngine
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.Hl7ReaderProvider;

        private readonly IRepository<Import, Guid> _importRepository;
        private readonly IHl7Process _hl7Process;
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly RecordProcessingService _recordProcessingService;

        public Hl7FileReader(
            IRepository<Import, Guid> import, 
            IHl7Process hl7Process,
            IPremiersoftHackathonDbContext dbContext,
            RecordProcessingService recordProcessingService)
        {
            _hl7Process = hl7Process;
            _importRepository = import;
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

            // Processa o HL7 usando o parser existente mas com nova lógica de salvamento
            await ProcessHl7WithRecordByRecord(import);
        }

        private async Task ProcessHl7WithRecordByRecord(Import import)
        {
            try
            {
                var xml = File.ReadAllText(import.S3PreSignedUrl);
                var hl7Parser = new Hl7Parser();
                var parsedFiles = await hl7Parser.ParseHl7Async(import);

                // Processa os arquivos HL7 registro por registro
                _hl7Process.Process(parsedFiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar HL7 para import {import.Id}: {ex.Message}");
                throw;
            }
        }
    }
}
