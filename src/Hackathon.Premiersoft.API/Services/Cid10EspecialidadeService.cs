using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Services
{
    /// <summary>
    /// Serviço para popular a tabela de relacionamento CID-10 x Especialidade
    /// com mapeamentos comuns baseados na classificação oficial do governo brasileiro
    /// </summary>
    public class Cid10EspecialidadeService
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;

        public Cid10EspecialidadeService(IPremiersoftHackathonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Popula a tabela com mapeamentos CID-10 x Especialidade comuns
        /// </summary>
        public async Task PopularMapeamentosCid10EspecialidadeAsync()
        {
            // Verificar se já existem mapeamentos
            var existingMappings = await _dbContext.Cid10Especialidades.AnyAsync();
            if (existingMappings)
                return;

            var mapeamentos = new List<(string CidCodigo, string Especialidade, int Prioridade, bool Primaria, string Observacoes)>
            {
                // Doenças Cardiovasculares
                ("I20", "Cardiologia", 1, true, "Angina do peito"),
                ("I21", "Cardiologia", 1, true, "Infarto agudo do miocárdio"),
                ("I25", "Cardiologia", 1, true, "Doença isquêmica crônica do coração"),
                ("I10", "Cardiologia", 1, true, "Hipertensão arterial"),
                
                // Doenças Respiratórias
                ("J44", "Pneumologia", 1, true, "DPOC - Doença pulmonar obstrutiva crônica"),
                ("J45", "Pneumologia", 1, true, "Asma"),
                ("J18", "Pneumologia", 1, true, "Pneumonia"),
                ("J20", "Pneumologia", 1, true, "Bronquite aguda"),
                
                // Doenças Neurológicas
                ("G40", "Neurologia", 1, true, "Epilepsia"),
                ("G35", "Neurologia", 1, true, "Esclerose múltipla"),
                ("I64", "Neurologia", 1, true, "AVC não especificado"),
                ("G20", "Neurologia", 1, true, "Doença de Parkinson"),
                
                // Doenças Psiquiátricas
                ("F32", "Psiquiatria", 1, true, "Episódio depressivo"),
                ("F20", "Psiquiatria", 1, true, "Esquizofrenia"),
                ("F41", "Psiquiatria", 1, true, "Outros transtornos ansiosos"),
                ("F10", "Psiquiatria", 1, true, "Transtornos mentais devido ao álcool"),
                
                // Doenças Ortopédicas
                ("M25", "Ortopedia", 1, true, "Outras artropatias"),
                ("M54", "Ortopedia", 1, true, "Dorsalgia"),
                ("S72", "Ortopedia", 1, true, "Fratura do fêmur"),
                ("M79", "Ortopedia", 1, true, "Outros transtornos dos tecidos moles"),
                
                // Doenças Gastrointestinais
                ("K29", "Gastroenterologia", 1, true, "Gastrite e duodenite"),
                ("K59", "Gastroenterologia", 1, true, "Outros transtornos funcionais do intestino"),
                ("K80", "Gastroenterologia", 1, true, "Colelitíase"),
                ("K92", "Gastroenterologia", 1, true, "Outras doenças do aparelho digestivo"),
                
                // Doenças Endócrinas
                ("E10", "Endocrinologia", 1, true, "Diabetes mellitus tipo 1"),
                ("E11", "Endocrinologia", 1, true, "Diabetes mellitus tipo 2"),
                ("E78", "Endocrinologia", 1, true, "Distúrbios do metabolismo de lipoproteínas"),
                ("E03", "Endocrinologia", 1, true, "Hipotireoidismo"),
                
                // Doenças Dermatológicas
                ("L20", "Dermatologia", 1, true, "Dermatite atópica"),
                ("L40", "Dermatologia", 1, true, "Psoríase"),
                ("L50", "Dermatologia", 1, true, "Urticária"),
                ("C44", "Dermatologia", 1, true, "Outras neoplasias malignas da pele"),
                
                // Doenças Urológicas
                ("N20", "Urologia", 1, true, "Cálculo do rim e do ureter"),
                ("N40", "Urologia", 1, true, "Hiperplasia da próstata"),
                ("N18", "Nefrologia", 1, true, "Doença renal crônica"),
                ("N39", "Urologia", 1, true, "Outros transtornos do trato urinário"),
                
                // Doenças Ginecológicas
                ("N92", "Ginecologia", 1, true, "Menstruação excessiva, frequente e irregular"),
                ("N80", "Ginecologia", 1, true, "Endometriose"),
                ("C50", "Oncologia", 1, true, "Neoplasia maligna da mama"),
                
                // Emergências - Clínica Geral como alternativa
                ("R50", "Clínica Geral", 1, true, "Febre não especificada"),
                ("R06", "Clínica Geral", 1, true, "Anormalidades da respiração"),
                ("R10", "Clínica Geral", 1, true, "Dor abdominal e pélvica"),
                ("Z00", "Clínica Geral", 1, true, "Exame geral e investigação de pessoas sem queixas"),
                
                // Especialidades secundárias para casos complexos
                ("I21", "Clínica Geral", 2, false, "Estabilização inicial antes da cardiologia"),
                ("J18", "Clínica Geral", 2, false, "Pneumonia simples pode ser tratada na clínica geral"),
                ("E11", "Clínica Geral", 2, false, "Diabetes estável pode ser acompanhado na clínica geral")
            };

            var cid10List = await _dbContext.Cid10.ToListAsync();
            var cid10Dict = cid10List.ToDictionary(c => c.Codigo, c => c.Id);

            foreach (var (cidCodigo, especialidade, prioridade, primaria, observacoes) in mapeamentos)
            {
                if (cid10Dict.TryGetValue(cidCodigo, out var cid10Id))
                {
                    await _dbContext.Cid10Especialidades.AddAsync(new Cid10Especialidade
                    {
                        Cid10Id = cid10Id,
                        Especialidade = especialidade,
                        Prioridade = prioridade,
                        EspecialidadePrimaria = primaria,
                        Observacoes = observacoes
                    });
                }
            }

            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        /// <summary>
        /// Busca especialidades recomendadas para um código CID-10
        /// </summary>
        public async Task<List<string>> BuscarEspecialidadesPorCidAsync(string cidCodigo)
        {
            var especialidades = await _dbContext.Cid10Especialidades
                .Include(ce => ce.Cid10)
                .Where(ce => ce.Cid10.Codigo == cidCodigo)
                .OrderBy(ce => ce.Prioridade)
                .Select(ce => ce.Especialidade)
                .ToListAsync();

            return especialidades;
        }

        /// <summary>
        /// Estatísticas dos mapeamentos CID-Especialidade
        /// </summary>
        public async Task<object> ObterEstatisticasMapeamentosAsync()
        {
            var totalMapeamentos = await _dbContext.Cid10Especialidades.CountAsync();
            var especialidadesPorCid = await _dbContext.Cid10Especialidades
                .GroupBy(ce => ce.Cid10Id)
                .Select(g => new { CidId = g.Key, Count = g.Count() })
                .ToListAsync();

            var especialidadesComuns = await _dbContext.Cid10Especialidades
                .GroupBy(ce => ce.Especialidade)
                .Select(g => new { Especialidade = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            return new
            {
                TotalMapeamentos = totalMapeamentos,
                MediaEspecialidadesPorCid = especialidadesPorCid.Average(x => x.Count),
                EspecialidadesMaisComuns = especialidadesComuns
            };
        }
    }
}