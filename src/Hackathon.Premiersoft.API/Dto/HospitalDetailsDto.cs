namespace Hackathon.Premiersoft.API.Dto
{
    public class HospitalDetailsDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public int LeitosTotal { get; set; }
        public int LeitosOcupados { get; set; }
        public int MedicosAlocados { get; set; }
        public decimal TaxaOcupacao { get; set; }
        public int RankingRegional { get; set; }
        public string TipoHospital { get; set; } = string.Empty;
    }

    public class HospitalSpecialtyDto
    {
        public string Especialidade { get; set; } = string.Empty;
        public int NumeroMedicos { get; set; }
        public int NumeroPacientes { get; set; }
    }

    public class HospitalListDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
    }
}