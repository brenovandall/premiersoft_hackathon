using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;

namespace Hackathon.Premiersoft.API.Repository.Municipios
{
    public class MunicipiosRepository : IMunicipiosRepository
    {
        private readonly IPremiersoftHackathonDbContext _context;
        public MunicipiosRepository(IPremiersoftHackathonDbContext context) => _context = context;
        public Models.Municipios GetByIbgeCode(string codigoIbge)
        {
            var municipio = _context.Set<Models.Municipios>().FirstOrDefault(x => x.Codigo_ibge == codigoIbge);
            return municipio is null ? throw new Exception("Não foi encontrado o municipio com o código ibge informado") : municipio;
        }

        public Models.Municipios GetMunicipioById(Guid id)
        {
            var municipio = _context.Set<Models.Municipios>().FirstOrDefault(x => x.Id == id);
            return municipio is null ? throw new Exception("Não foi encontrado o municipio com o código ibge informado") : municipio;
        }
    }
}
