using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Models.Abstractions;
using System.Threading;

public class GenericDataInsertEngine<TEntity, TId>
    where TEntity : Entity<TId>, new()
{
    private readonly IRepository<TEntity, TId> _repository;
    private readonly IPremiersoftHackathonDbContext _dbContext;

    public GenericDataInsertEngine(IRepository<TEntity, TId> repository, IPremiersoftHackathonDbContext dbContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<DataProcessingResult<TEntity>> ProcessAndInsertAsync(
        IEnumerable<CsvRow> rows,
        Dictionary<string, string> columnMapping,
        CancellationToken cancellationToken = default)
    {
        var result = new DataProcessingResult<TEntity> { TotalRowsProcessed = rows.Count() };
        var entitiesToInsert = new List<TEntity>();

        var entityProperties = typeof(TEntity).GetProperties()
            .ToDictionary(p => p.Name, p => p);

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = new TEntity();

            foreach (var mapping in columnMapping)
            {
                var sourceColumn = mapping.Key;
                var targetProperty = mapping.Value;

                if (!row.Data.TryGetValue(sourceColumn, out object? sourceValue))
                {
                    continue;
                }

                if (entityProperties.TryGetValue(targetProperty, out PropertyInfo? propInfo))
                {
                    try
                    {
                        var convertedValue = ConvertValue(sourceValue, propInfo.PropertyType);
                        propInfo.SetValue(entity, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new DataProcessingError
                        {
                            RowIndex = row.Index,
                            ColumnName = sourceColumn,
                            RawValue = sourceValue,
                            ErrorMessage = $"Erro de conversão para '{targetProperty}': {ex.Message}"
                        });

                        // Continua o processo mesmo com erro, não marca como inválido
                    }
                }
            }

            entitiesToInsert.Add(entity); // Sempre adiciona a entidade, mesmo com erros
        }

        if (entitiesToInsert.Any())
        {
            foreach (var entity in entitiesToInsert)
            {
                _repository.Add(entity);
            }

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                result.SuccessfulInserts.AddRange(entitiesToInsert);
            }
            catch (Exception ex)
            {
                // Falha na persistência (ex: restrições de banco, etc.)
                foreach (var entity in entitiesToInsert)
                {
                    result.Errors.Add(new DataProcessingError
                    {
                        RowIndex = -1,
                        ColumnName = string.Empty,
                        RawValue = string.Empty,
                        ErrorMessage = $"Erro ao salvar entidade no banco de dados: {ex.Message}"
                    });
                }
            }
        }

        return result;
    }


    private object? ConvertValue(object? sourceValue, Type targetType)
    {
        if (sourceValue == null || (sourceValue is string s && string.IsNullOrWhiteSpace(s)))
        {
            if (targetType.IsClass || Nullable.GetUnderlyingType(targetType) != null)
                return null;

            throw new InvalidCastException("Não é possível atribuir um valor nulo a um tipo não anulável.");
        }

        var nonNullableTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        var converter = TypeDescriptor.GetConverter(nonNullableTargetType);

        if (converter.CanConvertFrom(sourceValue.GetType()))
        {
            return converter.ConvertFrom(null, CultureInfo.InvariantCulture, sourceValue);
        }

        return Convert.ChangeType(sourceValue, nonNullableTargetType, CultureInfo.InvariantCulture);
    }
}
