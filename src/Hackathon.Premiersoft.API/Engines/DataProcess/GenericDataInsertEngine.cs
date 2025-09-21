// Engines/DataProcess/GenericDataInsertEngine.cs

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Repository; // Namespace do IRepository
using Hackathon.Premiersoft.API.Models.Abstractions; // Namespace do Entity<Tid>

// A restrição "where" foi atualizada para corresponder à do IRepository
public class GenericDataInsertEngine<TEntity, TId>
    where TEntity : Entity<TId>, new() // Adicionado Entity<TId> e mantido new()
{
    private readonly IRepository<TEntity, TId> _repository;
 
    public GenericDataInsertEngine(IRepository<TEntity, TId> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
     }

    public async Task<DataProcessingResult<TEntity>> ProcessAndInsertAsync(
        IEnumerable<CsvRow> rows,
        Dictionary<string, string> columnMapping)
    {
        var result = new DataProcessingResult<TEntity> { TotalRowsProcessed = rows.Count() };
        var entitiesToInsert = new List<TEntity>();

        var entityProperties = typeof(TEntity).GetProperties()
            .ToDictionary(p => p.Name, p => p);

        foreach (var row in rows)
        {
            var entity = new TEntity();
            bool isRowValid = true;

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
                        isRowValid = false;
                        // O 'break' foi comentado para registrar todos os erros da linha, não apenas o primeiro.
                        // break; 
                    }
                }
            }

            if (isRowValid)
            {
                entitiesToInsert.Add(entity);
            }
        }

        // Se houver entidades válidas, adiciona uma por uma usando o repositório
        if (entitiesToInsert.Any())
        {
            foreach (var entity in entitiesToInsert)
            {
                _repository.Add(entity);
            }

            // Salva todas as mudanças de uma vez usando a Unidade de Trabalho
          //  await _unitOfWork.SaveChangesAsync();
            result.SuccessfulInserts.AddRange(entitiesToInsert);
        }

        return result;
    }

    // O método ConvertValue permanece exatamente o mesmo
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