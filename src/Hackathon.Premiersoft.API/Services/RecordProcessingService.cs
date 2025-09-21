using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Hackathon.Premiersoft.API.Services
{
    public class RecordProcessingService
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;

        public RecordProcessingService(IPremiersoftHackathonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Processa um registro individual e salva no banco sem controle de transação
        /// </summary>
        /// <typeparam name="T">Tipo da entidade a ser salva</typeparam>
        /// <param name="entity">Entidade a ser salva</param>
        /// <param name="importId">ID da importação</param>
        /// <param name="lineNumber">Número da linha sendo processada</param>
        /// <param name="rawData">Dados brutos da linha para log de erro</param>
        /// <returns>True se salvou com sucesso, False se houve erro</returns>
        public async Task<bool> ProcessRecordAsync<T>(T entity, Guid importId, long lineNumber, Dictionary<string, object> rawData) where T : class
        {
            try
            {
                // Adiciona a entidade ao contexto
                _dbContext.Set<T>().Add(entity);
                
                // Salva imediatamente sem transação
                await _dbContext.SaveChangesAsync(CancellationToken.None);
                
                return true;
            }
            catch (Exception ex)
            {
                // Remove a entidade que causou erro do contexto para evitar problemas
                // Como não temos acesso ao Entry através da interface, vamos tentar uma abordagem diferente
                try
                {
                    // Tenta resetar o contexto se possível
                    if (_dbContext is DbContext dbContext)
                    {
                        var entry = dbContext.Entry(entity);
                        if (entry != null)
                        {
                            entry.State = EntityState.Detached;
                        }
                    }
                }
                catch
                {
                    // Se não conseguir remover do contexto, continua sem falhar
                }

                // Registra o erro na tabela LineError
                await LogLineErrorAsync(importId, lineNumber, ex, rawData);
                
                return false;
            }
        }

        /// <summary>
        /// Processa múltiplos registros de uma vez (para casos específicos onde é necessário)
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="entities">Lista de entidades</param>
        /// <param name="importId">ID da importação</param>
        /// <param name="startLineNumber">Número da primeira linha</param>
        /// <param name="rawDataList">Lista de dados brutos correspondentes</param>
        /// <returns>Número de registros processados com sucesso</returns>
        public async Task<int> ProcessRecordsBatchAsync<T>(List<T> entities, Guid importId, long startLineNumber, List<Dictionary<string, object>> rawDataList) where T : class
        {
            int successCount = 0;
            
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var lineNumber = startLineNumber + i;
                var rawData = i < rawDataList.Count ? rawDataList[i] : new Dictionary<string, object>();
                
                var success = await ProcessRecordAsync(entity, importId, lineNumber, rawData);
                if (success)
                {
                    successCount++;
                }
            }
            
            return successCount;
        }

        /// <summary>
        /// Atualiza contadores do Import
        /// </summary>
        /// <param name="importId">ID da importação</param>
        /// <param name="totalProcessed">Total de registros processados</param>
        /// <param name="totalSuccess">Total de registros salvos com sucesso</param>
        /// <param name="totalFailed">Total de registros que falharam</param>
        /// <param name="totalDuplicated">Total de registros duplicados</param>
        public async Task UpdateImportCountersAsync(Guid importId, int totalProcessed, int totalSuccess, int totalFailed, int totalDuplicated = 0)
        {
            try
            {
                var import = await _dbContext.Imports.FirstOrDefaultAsync(i => i.Id == importId);
                if (import != null)
                {
                    // Usa reflection para atualizar as propriedades privadas
                    var totalRegistersProperty = typeof(Import).GetProperty("TotalRegisters");
                    var totalImportedProperty = typeof(Import).GetProperty("TotalImportedRegisters");
                    var totalFailedProperty = typeof(Import).GetProperty("TotalFailedRegisters");
                    var totalDuplicatedProperty = typeof(Import).GetProperty("TotalDuplicatedRegisters");
                    var finishedOnProperty = typeof(Import).GetProperty("FinishedOn");

                    totalRegistersProperty?.SetValue(import, totalProcessed);
                    totalImportedProperty?.SetValue(import, totalSuccess);
                    totalFailedProperty?.SetValue(import, totalFailed);
                    totalDuplicatedProperty?.SetValue(import, totalDuplicated);
                    finishedOnProperty?.SetValue(import, DateTime.UtcNow);

                    await _dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar contadores do import {importId}: {ex.Message}");
            }
        }

        private async Task LogLineErrorAsync(Guid importId, long lineNumber, Exception ex, Dictionary<string, object> rawData)
        {
            try
            {
                // Determina qual campo causou o erro (análise básica da exceção)
                var field = ExtractFieldFromException(ex, rawData);
                var value = ExtractValueFromException(ex, rawData, field);

                var lineError = new LineError(
                    importId: importId,
                    import: null!, // Não carregamos o import completo para performance
                    line: lineNumber,
                    field: field,
                    error: ex.Message,
                    value: value);

                _dbContext.LineErrors.Add(lineError);
                await _dbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception logEx)
            {
                // Se não conseguir salvar o erro, pelo menos logga no console
                Console.WriteLine($"Erro ao salvar LineError: {logEx.Message}. Erro original: {ex.Message}");
            }
        }

        private string ExtractFieldFromException(Exception ex, Dictionary<string, object> rawData)
        {
            // Tenta extrair o nome do campo da mensagem de erro
            var message = ex.Message.ToLowerInvariant();
            
            foreach (var kvp in rawData)
            {
                if (message.Contains(kvp.Key.ToLowerInvariant()))
                {
                    return kvp.Key;
                }
            }

            // Se não conseguiu identificar, retorna um valor genérico
            return "unknown_field";
        }

        private string ExtractValueFromException(Exception ex, Dictionary<string, object> rawData, string field)
        {
            // Tenta pegar o valor do campo que causou erro
            if (rawData.TryGetValue(field, out var value))
            {
                return value?.ToString() ?? "null";
            }

            // Se não encontrou, pega o primeiro valor não nulo
            var firstValue = rawData.Values.FirstOrDefault(v => v != null);
            return firstValue?.ToString() ?? "unknown_value";
        }

        /// <summary>
        /// Valida se uma entidade é duplicada baseada em campos únicos
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="entity">Entidade a validar</param>
        /// <param name="uniqueFields">Campos que definem unicidade</param>
        /// <returns>True se é duplicada</returns>
        public async Task<bool> IsDuplicateAsync<T>(T entity, string[] uniqueFields) where T : class
        {
            try
            {
                var entityType = typeof(T);
                var dbSet = _dbContext.Set<T>();
                
                // Cria uma query dinâmica baseada nos campos únicos
                IQueryable<T> query = dbSet;
                
                foreach (var fieldName in uniqueFields)
                {
                    var property = entityType.GetProperty(fieldName);
                    if (property != null)
                    {
                        var value = property.GetValue(entity);
                        if (value != null)
                        {
                            // Aplica filtro para cada campo único
                            query = query.Where(e => EF.Property<object>(e, fieldName).Equals(value));
                        }
                    }
                }
                
                return await query.AnyAsync();
            }
            catch
            {
                return false; // Em caso de erro, considera como não duplicado
            }
        }
    }
}