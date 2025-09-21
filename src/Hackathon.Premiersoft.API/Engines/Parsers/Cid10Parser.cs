using Hackathon.Premiersoft.API.Models;
using System.Text.RegularExpressions;

namespace Hackathon.Premiersoft.API.Engines.Parsers
{
    public class Cid10Parser
    {
        private readonly Regex _cid10Pattern;
        
        public Cid10Parser()
        {
            // Padrão para códigos CID-10: 1 letra + 2 dígitos seguido de " - " e descrição
            _cid10Pattern = new Regex(@"^([A-Z]\d{2})\s*-\s*(.+)$", RegexOptions.Compiled);
        }

        /// <summary>
        /// Processa um arquivo CSV de CID-10 e extrai apenas códigos válidos
        /// </summary>
        /// <param name="filePath">Caminho para o arquivo CSV</param>
        /// <returns>Lista de objetos Cid10 válidos</returns>
        public async Task<List<Cid10>> ParseCid10FromFileAsync(string filePath)
        {
            var cid10List = new List<Cid10>();
            
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                
                foreach (var line in lines)
                {
                    var cid10 = ParseCid10FromLine(line);
                    if (cid10 != null)
                    {
                        cid10List.Add(cid10);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar arquivo CID-10: {ex.Message}", ex);
            }
            
            return cid10List;
        }

        /// <summary>
        /// Processa um TextReader de CID-10 e extrai apenas códigos válidos
        /// </summary>
        /// <param name="reader">TextReader do arquivo</param>
        /// <returns>Lista de objetos Cid10 válidos</returns>
        public async Task<List<Cid10>> ParseCid10FromReaderAsync(TextReader reader)
        {
            var cid10List = new List<Cid10>();
            
            try
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var cid10 = ParseCid10FromLine(line);
                    if (cid10 != null)
                    {
                        cid10List.Add(cid10);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar dados CID-10: {ex.Message}", ex);
            }
            
            return cid10List;
        }

        /// <summary>
        /// Processa uma linha e extrai código CID-10 se válido
        /// </summary>
        /// <param name="line">Linha do arquivo</param>
        /// <returns>Objeto Cid10 se válido, null caso contrário</returns>
        private Cid10? ParseCid10FromLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            // Remove aspas duplas se existirem
            line = line.Trim().Trim('"');

            var match = _cid10Pattern.Match(line);
            
            if (!match.Success)
                return null;

            var codigo = match.Groups[1].Value.Trim();
            var descricao = match.Groups[2].Value.Trim();

            // Validação adicional: código deve ter exatamente 1 letra + 2 dígitos
            if (codigo.Length != 3 || !char.IsLetter(codigo[0]) || !char.IsDigit(codigo[1]) || !char.IsDigit(codigo[2]))
                return null;

            // Remove caracteres especiais da descrição, mantendo apenas letras, números, espaços e pontuação básica
            descricao = CleanDescription(descricao);

            if (string.IsNullOrWhiteSpace(descricao))
                return null;

            return new Cid10
            {
                Codigo = codigo,
                Descricao = descricao
            };
        }

        /// <summary>
        /// Remove caracteres especiais da descrição, mantendo apenas caracteres permitidos
        /// </summary>
        /// <param name="description">Descrição original</param>
        /// <returns>Descrição limpa</returns>
        private string CleanDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return string.Empty;

            // Remove caracteres especiais mantendo letras, números, espaços e pontuação básica
            var cleanPattern = new Regex(@"[^\w\s\[\]\(\)\-\.,;:]", RegexOptions.Compiled);
            var cleaned = cleanPattern.Replace(description, "");
            
            // Remove espaços múltiplos
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
            
            return cleaned;
        }

        /// <summary>
        /// Valida se um código está no formato CID-10 correto
        /// </summary>
        /// <param name="codigo">Código a ser validado</param>
        /// <returns>True se válido, false caso contrário</returns>
        public bool IsValidCid10Code(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 3)
                return false;

            return char.IsLetter(codigo[0]) && char.IsDigit(codigo[1]) && char.IsDigit(codigo[2]);
        }
    }
}