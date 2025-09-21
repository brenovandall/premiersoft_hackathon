using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Models
{
    public class Cid10 : Entity<Guid>
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public Cid10()
        {
            Id = Guid.NewGuid();
        }
    }
}
