// Dto/DataProcessingResult.cs

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Representa um erro detalhado ocorrido durante o processamento de uma linha.
/// </summary>
public class DataProcessingError
{
    public int RowIndex { get; set; }
    public string? ColumnName { get; set; }
    public object? RawValue { get; set; }
    public string ErrorMessage { get; set; }
}

/// <summary>
/// Encapsula o resultado de uma operação de processamento e inserção de dados.
/// </summary>
/// <typeparam name="T">O tipo da entidade que foi processada.</typeparam>
public class DataProcessingResult<T> where T : class
{
    public List<T> SuccessfulInserts { get; private set; } = new List<T>();
    public List<DataProcessingError> Errors { get; private set; } = new List<DataProcessingError>();
    public int TotalRowsProcessed { get; set; }
    public int SuccessCount => SuccessfulInserts.Count;
    public int ErrorCount => Errors.Count;
    public bool HasErrors => Errors.Any();
}