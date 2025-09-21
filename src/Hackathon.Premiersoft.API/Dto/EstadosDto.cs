namespace Hackathon.Premiersoft.API.Dto
{
    public class EstadosDto
    {
        public Guid Id { get; set; }
        public int Codigo_uf { get; set; }
        public string Uf { get; set; }
        public string Nome { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Regiao { get; set; }
    }
}