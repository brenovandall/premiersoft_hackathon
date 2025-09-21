using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Services;

namespace Hackathon.Premiersoft.API.Engines.Parsers.Hl7
{
    public class Hl7Process : IHl7Process
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly RecordProcessingService _recordProcessingService;

        public Hl7Process(
            IPremiersoftHackathonDbContext dbContext,
            RecordProcessingService recordProcessingService)
        {
            _dbContext = dbContext;
            _recordProcessingService = recordProcessingService;
        }

        public void Process(List<FileHl7Dto> hl7Files)
        {
            Console.WriteLine($"Processando {hl7Files.Count} registros HL7");

            foreach (var hl7File in hl7Files)
            {
                try
                {
                    // TODO: Implementar a lógica específica de processamento HL7
                    // Por enquanto, apenas logga as informações
                    Console.WriteLine($"Linha {hl7File.NumeroLinha}: {hl7File.Campo} = {hl7File.Valor}");
                    
                    // Aqui você pode implementar a lógica de:
                    // 1. Mapear dados HL7 para entidades específicas
                    // 2. Validar dados
                    // 3. Salvar usando o RecordProcessingService
                    // 4. Registrar erros em LineError se necessário
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar registro HL7 linha {hl7File.NumeroLinha}: {ex.Message}");
                }
            }
        }
    }
}