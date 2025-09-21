namespace Hackathon.Premiersoft.API.Dto
{
    public class PacientesDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Cpf { get; set; }
        public string Genero { get; set; }
        public string Nome_completo { get; set; }
        public bool Convenio { get; set; }
        public string Municipio { get; set; }
        public string Cid10 { get; set; }
        public string DescricaoCid10 { get; set; }
    }
}