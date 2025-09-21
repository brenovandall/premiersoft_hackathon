using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Helpers;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Engines.Xml;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class XmlFileReader : IFileReaderEngine
    {
        private readonly IRepository<Import, Guid> _importRepository;
        private readonly IXmlParser _xmlParser;
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly RecordProcessingService _recordProcessingService;

        public string FileReaderProvider => Extensions.FileReaderProvider.XmlReaderProvider;

        public XmlFileReader(
            IRepository<Import, Guid> importsRepo, 
            IXmlParser xmlParser,
            IPremiersoftHackathonDbContext dbContext,
            RecordProcessingService recordProcessingService)
        {
            _xmlParser = xmlParser;
            _importRepository = importsRepo;
            _dbContext = dbContext;
            _recordProcessingService = recordProcessingService;
        }

        public async void Run(Guid importId)
        {
            var contador = new Stopwatch();
            contador.Start();

            var import = await _dbContext.Imports.FirstOrDefaultAsync(i => i.Id == importId);
            if (import == null)
            {
                throw new Exception($"Import com ID {importId} não encontrado");
            }

            if (string.IsNullOrEmpty(import.S3PreSignedUrl))
                throw new Exception("URL do arquivo não encontrado!");

            // Processa o XML usando o parser existente mas com nova lógica de salvamento
            await ProcessXmlWithRecordByRecord(import);
            
            contador.Stop();
            Console.WriteLine($"Processamento XML concluído em {contador.Elapsed.Seconds} segundos");
        }

        private async Task ProcessXmlWithRecordByRecord(Import import)
        {
            try
            {
                // Usa o parser existente mas modifica para processar registro por registro
                await _xmlParser.ParseXmlAsync(import);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar XML para import {import.Id}: {ex.Message}");
                throw;
            }
        }
    }
}
