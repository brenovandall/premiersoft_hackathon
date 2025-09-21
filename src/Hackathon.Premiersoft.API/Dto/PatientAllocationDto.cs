namespace Hackathon.Premiersoft.API.Dto
{
    /// <summary>
    /// DTO para representar a alocação de um paciente em um hospital
    /// com todas as informações relevantes para decisões operacionais
    /// </summary>
    public class PatientAllocationDto
    {
        public Guid Id { get; set; }
        
        // Informações do Paciente
        public string PacienteNome { get; set; } = default!;
        public string PacienteCpf { get; set; } = default!;
        public string PacienteMunicipio { get; set; } = default!;
        public string Regiao { get; set; } = default!;
        
        // Informações do CID-10
        public string CidCodigo { get; set; } = default!;
        public string CidDescricao { get; set; } = default!;
        
        // Informações do Hospital
        public string HospitalNome { get; set; } = default!;
        public string HospitalCodigo { get; set; } = default!;
        public string HospitalEspecialidades { get; set; } = default!;
        public string HospitalMunicipio { get; set; } = default!;
        
        // Informações da Alocação
        public double DistanciaKm { get; set; }
        public string Status { get; set; } = default!;
        public DateTime DataAlocacao { get; set; }
        public int Prioridade { get; set; }
        public string? Observacoes { get; set; }
    }
    
    /// <summary>
    /// DTO para filtros de busca de alocações
    /// </summary>
    public class PatientAllocationFilterDto
    {
        public string? Regiao { get; set; }
        public string? Hospital { get; set; }
        public string? Especialidade { get; set; }
        public string? Status { get; set; }
        public string? CidCodigo { get; set; }
        public DateTime? DataInicioAlocacao { get; set; }
        public DateTime? DataFimAlocacao { get; set; }
        public double? DistanciaMaxima { get; set; }
        public int? Prioridade { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
    
    /// <summary>
    /// DTO para resumo de alocações por especialidade
    /// </summary>
    public class AllocationSummaryBySpecialtyDto
    {
        public string Especialidade { get; set; } = default!;
        public int TotalPacientes { get; set; }
        public double DistanciaMedia { get; set; }
        public int PacientesAlocados { get; set; }
        public int PacientesTransferidos { get; set; }
        public int PacientesAlta { get; set; }
    }
}