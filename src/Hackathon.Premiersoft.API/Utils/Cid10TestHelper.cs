using Hackathon.Premiersoft.API.Engines.Parsers;
using Hackathon.Premiersoft.API.Models;

// Exemplo de teste do parser CID-10
// Este arquivo pode ser usado para testar a funcionalidade localmente

namespace Hackathon.Premiersoft.API.Utils
{
    public static class Cid10TestHelper
    {
        /// <summary>
        /// Testa o parser CID-10 com dados de exemplo
        /// </summary>
        /// <returns>Lista de códigos CID-10 processados</returns>
        public static async Task<List<Cid10>> TestCid10Parser()
        {
            var parser = new Cid10Parser();
            
            // Dados de exemplo baseados no formato do arquivo fornecido
            var testData = @"CID-10
Total de Concessões com CID-10 Identificada
Capítulo I: Algumas doenças Infecciosas e parasitárias (A00-B99)
A00 - Cólera
A01 - Febres tifóide e paratifóide
A02 - Outras infecções por Salmonella
A03 - Shiguelose
A04 - Outras infecções intestinais bacterianas
A05 - Outras intoxicações alimentares bacterianas
A06 - Amebíase
""A08 - Infecções intestinais virais, outras e as não especificadas""
A09 - Diarréia e gastroenterite de origem infecciosa presumível
""A15 - Tuberculose respiratória, com confirmação bacteriológica e histológica""
""A16 - Tuberculose das vias respiratórias, sem confirmação bacteriológica ou histológica""
A17 - Tuberculose do sistema nervoso
A18 - Tuberculose de outros órgãos
A19 - Tuberculose miliar
B22 - Doença pelo vírus da imunodeficiência humana [HIV] resultando em outras doenças especificadas
B23 - Doença pelo vírus da imunodeficiência humana [HIV] resultando em outras doenças
Z99 - Dependência de máquinas e dispositivos capacitantes não classificados em outra parte";

            using var reader = new StringReader(testData);
            var result = await parser.ParseCid10FromReaderAsync(reader);
            
            return result;
        }

        /// <summary>
        /// Valida a estrutura dos códigos CID-10 processados
        /// </summary>
        /// <param name="cid10List">Lista de códigos para validar</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateCid10List(List<Cid10> cid10List)
        {
            var result = new ValidationResult();
            
            foreach (var cid10 in cid10List)
            {
                // Validar formato do código
                if (string.IsNullOrWhiteSpace(cid10.Codigo) || cid10.Codigo.Length != 3)
                {
                    result.Errors.Add($"Código inválido: '{cid10.Codigo}' - deve ter exatamente 3 caracteres");
                    continue;
                }

                if (!char.IsLetter(cid10.Codigo[0]) || !char.IsDigit(cid10.Codigo[1]) || !char.IsDigit(cid10.Codigo[2]))
                {
                    result.Errors.Add($"Formato inválido: '{cid10.Codigo}' - deve ser 1 letra + 2 dígitos");
                    continue;
                }

                // Validar descrição
                if (string.IsNullOrWhiteSpace(cid10.Descricao))
                {
                    result.Errors.Add($"Descrição vazia para código: '{cid10.Codigo}'");
                    continue;
                }

                result.ValidCount++;
            }

            result.TotalCount = cid10List.Count;
            result.IsValid = result.Errors.Count == 0;
            
            return result;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public int TotalCount { get; set; }
        public int ValidCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public string GetSummary()
        {
            if (IsValid)
                return $"Todos os {TotalCount} códigos são válidos.";
            
            return $"Encontrados {Errors.Count} erros em {TotalCount} códigos. {ValidCount} códigos válidos.";
        }
    }
}