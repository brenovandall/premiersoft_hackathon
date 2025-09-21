namespace Hackathon.Premiersoft.API.Dto
{
    public class HospitaisDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public long Leitos_totais { get; set; }
    }
}