using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Parsers;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Services
{
    public interface ICid10ImportService
    {
        Task<Cid10ImportResult> ImportFromFileAsync(string filePath);
        Task<Cid10ImportResult> ImportFromReaderAsync(TextReader reader);
        Task<Cid10ImportResult> ImportCid10ListAsync(List<Cid10> cid10List);
    }

    public class Cid10ImportService : ICid10ImportService
    {
        private readonly IRepository<Cid10, Guid> _cid10Repository;
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly Cid10Parser _parser;

        public Cid10ImportService(
            IRepository<Cid10, Guid> cid10Repository,
            IPremiersoftHackathonDbContext dbContext)
        {
            _cid10Repository = cid10Repository;
            _dbContext = dbContext;
            _parser = new Cid10Parser();
        }

        public async Task<Cid10ImportResult> ImportFromFileAsync(string filePath)
        {
            try
            {
                var cid10List = await _parser.ParseCid10FromFileAsync(filePath);
                return await ImportCid10ListAsync(cid10List);
            }
            catch (Exception ex)
            {
                return new Cid10ImportResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao processar arquivo: {ex.Message}",
                    ProcessedCount = 0,
                    ImportedCount = 0,
                    SkippedCount = 0
                };
            }
        }

        public async Task<Cid10ImportResult> ImportFromReaderAsync(TextReader reader)
        {
            try
            {
                var cid10List = await _parser.ParseCid10FromReaderAsync(reader);
                return await ImportCid10ListAsync(cid10List);
            }
            catch (Exception ex)
            {
                return new Cid10ImportResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao processar dados: {ex.Message}",
                    ProcessedCount = 0,
                    ImportedCount = 0,
                    SkippedCount = 0
                };
            }
        }

        public async Task<Cid10ImportResult> ImportCid10ListAsync(List<Cid10> cid10List)
        {
            var result = new Cid10ImportResult
            {
                ProcessedCount = cid10List.Count,
                ImportedCount = 0,
                SkippedCount = 0,
                Success = true,
                ErrorMessage = null
            };

            try
            {
                // Obter códigos existentes no banco
                var existingCodes = await _dbContext.Set<Cid10>()
                    .Select(c => c.Codigo)
                    .ToListAsync();
                var existingCodesSet = existingCodes.ToHashSet();

                var newCid10Items = new List<Cid10>();

                foreach (var cid10 in cid10List)
                {
                    // Verifica se o código já existe
                    if (existingCodesSet.Contains(cid10.Codigo))
                    {
                        result.SkippedCount++;
                        continue;
                    }

                    // Verifica se já foi adicionado na lista atual (evita duplicatas no mesmo arquivo)
                    if (newCid10Items.Any(x => x.Codigo == cid10.Codigo))
                    {
                        result.SkippedCount++;
                        continue;
                    }

                    newCid10Items.Add(cid10);
                    result.ImportedCount++;
                }

                // Adiciona todos os novos registros em lote
                if (newCid10Items.Any())
                {
                    await _dbContext.Set<Cid10>().AddRangeAsync(newCid10Items);
                    await _dbContext.SaveChangesAsync(CancellationToken.None);
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Erro ao salvar no banco de dados: {ex.Message}";
                result.ImportedCount = 0;
            }

            return result;
        }
    }

    public class Cid10ImportResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int ProcessedCount { get; set; }
        public int ImportedCount { get; set; }
        public int SkippedCount { get; set; }

        public string GetSummary()
        {
            if (!Success)
                return $"Falha na importação: {ErrorMessage}";

            return $"Processados: {ProcessedCount}, Importados: {ImportedCount}, Ignorados: {SkippedCount}";
        }
    }
}