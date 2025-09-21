using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Csv;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class CsvFileReaderEngine : IFileReaderEngine
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly RecordProcessingService _recordProcessingService;
        private readonly IServiceScopeFactory _scopeFactory;

        public string FileReaderProvider => Extensions.FileReaderProvider.CsvReaderProvider;

        public CsvFileReaderEngine(
            IPremiersoftHackathonDbContext dbContext,
            RecordProcessingService recordProcessingService,
            IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _recordProcessingService = recordProcessingService;
            _scopeFactory = scopeFactory;
        }

        public async Task Run(Guid importId)
        {
            try
            {
                // Busca as informações do import
                var import = await _dbContext.Imports.FirstOrDefaultAsync(i => i.Id == importId);
                if (import == null)
                {
                    throw new Exception($"Import com ID {importId} não encontrado");
                }

                // Processa o arquivo CSV usando o S3PreSignedUrl do import
                await ProcessCsvFileAsync(import);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar arquivo CSV para import {importId}: {ex.Message}");
                throw;
            }
        }

        private async Task ProcessCsvFileAsync(Import import)
        {
            var s3Service = new S3Service();
            using var reader = await s3Service.ObterLeitorDoArquivoAsync(import.S3PreSignedUrl);

            var csvReader = new CsvFileReaderProcess(_scopeFactory);
            var csvData = await csvReader.ParseCsvDataAsync(reader);

            int totalProcessed = 0;
            int totalSuccess = 0;
            int totalFailed = 0;
            int totalDuplicated = 0;

            // Processa cada linha individualmente
            foreach (var row in csvData.Rows)
            {
                totalProcessed++;
                long lineNumber = row.Index + 2; // +2 porque index começa em 0 e temos header

                try
                {
                    // Determina o tipo de entidade baseado no DataType do import
                    var success = await ProcessRowBasedOnDataType(import, row, lineNumber);
                    
                    if (success)
                    {
                        totalSuccess++;
                    }
                    else
                    {
                        totalFailed++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar linha {lineNumber}: {ex.Message}");
                    totalFailed++;
                }
            }

            // Atualiza contadores do import
            await _recordProcessingService.UpdateImportCountersAsync(
                import.Id, totalProcessed, totalSuccess, totalFailed, totalDuplicated);

            Console.WriteLine($"Processamento concluído - Total: {totalProcessed}, Sucesso: {totalSuccess}, Falhas: {totalFailed}");
        }

        private async Task<bool> ProcessRowBasedOnDataType(Import import, CsvRow row, long lineNumber)
        {
            return import.DataType switch
            {
                Models.Enums.ImportDataTypes.City => await ProcessMunicipioRecord(import.Id, row, lineNumber),
                Models.Enums.ImportDataTypes.State => await ProcessEstadoRecord(import.Id, row, lineNumber),
                Models.Enums.ImportDataTypes.Doctor => await ProcessMedicoRecord(import.Id, row, lineNumber),
                Models.Enums.ImportDataTypes.Hospital => await ProcessHospitalRecord(import.Id, row, lineNumber),
                Models.Enums.ImportDataTypes.Patient => await ProcessPacienteRecord(import.Id, row, lineNumber),
                Models.Enums.ImportDataTypes.CidTable => await ProcessCidRecord(import.Id, row, lineNumber),
                _ => throw new NotSupportedException($"Tipo de dados {import.DataType} não suportado para CSV")
            };
        }

        private async Task<bool> ProcessMunicipioRecord(Guid importId, CsvRow row, long lineNumber)
        {
            try
            {
                // Mapeia os dados do CSV para a entidade Municipios
                var municipio = MapCsvToMunicipio(row);
                
                // Verifica duplicatas usando código IBGE
                var isDuplicate = await _recordProcessingService.IsDuplicateAsync(municipio, new[] { "Codigo_ibge" });
                if (isDuplicate)
                {
                    // Considera sucesso mas não salva
                    return true;
                }

                return await _recordProcessingService.ProcessRecordAsync(municipio, importId, lineNumber, row.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar município na linha {lineNumber}: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ProcessEstadoRecord(Guid importId, CsvRow row, long lineNumber)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Processando estado na linha {lineNumber}");
                var estado = MapCsvToEstado(row);
                Console.WriteLine($"[DEBUG] Estado mapeado: UF={estado.Uf}, Nome={estado.Nome}");
                
                var isDuplicate = await _recordProcessingService.IsDuplicateAsync(estado, new[] { "Uf" });
                if (isDuplicate) 
                {
                    Console.WriteLine($"[DEBUG] Estado duplicado detectado: {estado.Uf}");
                    return true;
                }

                var result = await _recordProcessingService.ProcessRecordAsync(estado, importId, lineNumber, row.Data);
                Console.WriteLine($"[DEBUG] Resultado do processamento: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erro ao processar estado na linha {lineNumber}: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                // Agora vamos chamar diretamente o LogLineErrorAsync
                await _recordProcessingService.LogLineErrorAsync(importId, lineNumber, ex, row.Data);
                return false;
            }
        }

        private async Task<bool> ProcessMedicoRecord(Guid importId, CsvRow row, long lineNumber)
        {
            try
            {
                var medico = MapCsvToMedico(row);
                var isDuplicate = await _recordProcessingService.IsDuplicateAsync(medico, new[] { "Codigo" });
                if (isDuplicate) return true;

                return await _recordProcessingService.ProcessRecordAsync(medico, importId, lineNumber, row.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar médico na linha {lineNumber}: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ProcessHospitalRecord(Guid importId, CsvRow row, long lineNumber)
        {
            try
            {
                var hospital = MapCsvToHospital(row);
                var isDuplicate = await _recordProcessingService.IsDuplicateAsync(hospital, new[] { "Codigo" });
                if (isDuplicate) return true;

                return await _recordProcessingService.ProcessRecordAsync(hospital, importId, lineNumber, row.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar hospital na linha {lineNumber}: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ProcessPacienteRecord(Guid importId, CsvRow row, long lineNumber)
        {
            try
            {
                var paciente = MapCsvToPaciente(row);
                var isDuplicate = await _recordProcessingService.IsDuplicateAsync(paciente, new[] { "Cpf" });
                if (isDuplicate) return true;

                return await _recordProcessingService.ProcessRecordAsync(paciente, importId, lineNumber, row.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar paciente na linha {lineNumber}: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ProcessCidRecord(Guid importId, CsvRow row, long lineNumber)
        {
            try
            {
                var cid = MapCsvToCid10(row);
                var isDuplicate = await _recordProcessingService.IsDuplicateAsync(cid, new[] { "Codigo" });
                if (isDuplicate) return true;

                return await _recordProcessingService.ProcessRecordAsync(cid, importId, lineNumber, row.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar CID10 na linha {lineNumber}: {ex.Message}");
                return false;
            }
        }

        // Métodos de mapeamento - implementar baseado na estrutura das entidades
        private Municipios MapCsvToMunicipio(CsvRow row)
        {
            return new Municipios
            {
                Codigo_ibge = GetStringValue(row, "codigo_ibge") ?? throw new ArgumentException("Código IBGE é obrigatório"),
                Nome = GetStringValue(row, "nome") ?? throw new ArgumentException("Nome é obrigatório"),
                Latitude = GetDecimalValue(row, "latitude") ?? 0,
                Longitude = GetDecimalValue(row, "longitude") ?? 0,
                Capital = GetBoolValue(row, "capital") ?? false,
                Codigo_uf = GetStringValue(row, "codigo_uf") ?? throw new ArgumentException("Código UF é obrigatório"),
                Siafi_id = GetIntValue(row, "siafi_id") ?? 0,
                Ddd = GetIntValue(row, "ddd") ?? 0,
                Fuso_horario = GetStringValue(row, "fuso_horario") ?? "",
                Populacao = GetIntValue(row, "populacao") ?? 0
            };
        }

        private Estados MapCsvToEstado(CsvRow row)
        {
            return new Estados
            {
                Codigo_uf = GetIntValue(row, "codigo_uf") ?? throw new ArgumentException("Código UF é obrigatório"),
                Uf = GetStringValue(row, "uf") ?? throw new ArgumentException("UF é obrigatória"),
                Nome = GetStringValue(row, "nome") ?? throw new ArgumentException("Nome é obrigatório"),
                Latitude = GetDecimalValue(row, "latitude") ?? 0,
                Longitude = GetDecimalValue(row, "longitude") ?? 0,
                Regiao = GetStringValue(row, "regiao") ?? throw new ArgumentException("Região é obrigatória")
            };
        }

        private Medicos MapCsvToMedico(CsvRow row)
        {
            return new Medicos
            {
                Codigo = GetStringValue(row, "codigo") ?? throw new ArgumentException("Código é obrigatório"),
                Nome_completo = GetStringValue(row, "nome_completo") ?? throw new ArgumentException("Nome completo é obrigatório"),
                Especialidade = GetStringValue(row, "especialidade") ?? throw new ArgumentException("Especialidade é obrigatória"),
                MunicipioId = GetGuidValue(row, "municipio_id") ?? throw new ArgumentException("ID do município é obrigatório")
            };
        }

        private Hospitais MapCsvToHospital(CsvRow row)
        {
            return new Hospitais
            {
                Codigo = GetStringValue(row, "codigo") ?? throw new ArgumentException("Código é obrigatório"),
                Nome = GetStringValue(row, "nome") ?? throw new ArgumentException("Nome é obrigatório"),
                Bairro = GetStringValue(row, "bairro") ?? "",
                Especialidades = GetStringValue(row, "especialidades") ?? throw new ArgumentException("Especialidade é obrigatória"),
                Leitos_totais = GetLongValue(row, "leitos_totais") ?? 0
                // Nota: Cidade será resolvida separadamente se necessário
            };
        }

        private Pacientes MapCsvToPaciente(CsvRow row)
        {
            return new Pacientes
            {
                Codigo = GetStringValue(row, "codigo") ?? throw new ArgumentException("Código é obrigatório"),
                Cpf = GetStringValue(row, "cpf") ?? throw new ArgumentException("CPF é obrigatório"),
                Genero = GetStringValue(row, "genero") ?? "",
                Nome_completo = GetStringValue(row, "nome_completo") ?? throw new ArgumentException("Nome completo é obrigatório"),
                Convenio = GetBoolValue(row, "convenio") ?? false,
                Codigo_MunicipioId = GetGuidValue(row, "codigo_municipio_id") ?? throw new ArgumentException("ID do município é obrigatório"),
                Cid10Id = GetGuidValue(row, "cid10_id") ?? throw new ArgumentException("ID do CID10 é obrigatório")
            };
        }

        private Cid10 MapCsvToCid10(CsvRow row)
        {
            return new Cid10
            {
                Codigo = GetStringValue(row, "codigo") ?? throw new ArgumentException("Código é obrigatório"),
                Descricao = GetStringValue(row, "descricao") ?? throw new ArgumentException("Descrição é obrigatória")
            };
        }

        private string? GetStringValue(CsvRow row, string columnName)
        {
            var normalizedName = NormalizeHeaderName(columnName);
            if (row.Data.TryGetValue(normalizedName, out var value))
            {
                return value?.ToString()?.Trim();
            }
            return null;
        }

        private DateTime? GetDateTimeValue(CsvRow row, string columnName)
        {
            var stringValue = GetStringValue(row, columnName);
            if (DateTime.TryParse(stringValue, out var dateValue))
            {
                return dateValue;
            }
            return null;
        }

        private decimal? GetDecimalValue(CsvRow row, string columnName)
        {
            var stringValue = GetStringValue(row, columnName);
            if (decimal.TryParse(stringValue, out var decimalValue))
            {
                return decimalValue;
            }
            return null;
        }

        private int? GetIntValue(CsvRow row, string columnName)
        {
            var stringValue = GetStringValue(row, columnName);
            if (int.TryParse(stringValue, out var intValue))
            {
                return intValue;
            }
            return null;
        }

        private long? GetLongValue(CsvRow row, string columnName)
        {
            var stringValue = GetStringValue(row, columnName);
            if (long.TryParse(stringValue, out var longValue))
            {
                return longValue;
            }
            return null;
        }

        private bool? GetBoolValue(CsvRow row, string columnName)
        {
            var stringValue = GetStringValue(row, columnName);
            if (bool.TryParse(stringValue, out var boolValue))
            {
                return boolValue;
            }
            // Tenta interpretar valores comuns para booleano
            if (!string.IsNullOrEmpty(stringValue))
            {
                var lowerValue = stringValue.ToLowerInvariant();
                if (lowerValue == "1" || lowerValue == "sim" || lowerValue == "yes" || lowerValue == "true")
                    return true;
                if (lowerValue == "0" || lowerValue == "não" || lowerValue == "no" || lowerValue == "false")
                    return false;
            }
            return null;
        }

        private Guid? GetGuidValue(CsvRow row, string columnName)
        {
            var stringValue = GetStringValue(row, columnName);
            if (Guid.TryParse(stringValue, out var guidValue))
            {
                return guidValue;
            }
            return null;
        }

        private string NormalizeHeaderName(string headerName)
        {
            return headerName
                .Replace(" ", "_")
                .Replace("-", "_")
                .ToLowerInvariant();
        }
    }
}