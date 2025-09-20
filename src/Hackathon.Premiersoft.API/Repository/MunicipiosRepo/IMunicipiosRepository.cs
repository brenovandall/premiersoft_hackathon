
namespace Hackathon.Premiersoft.API.Repository.MunicipiosRepo
{
    public interface IMunicipiosRepository
    {
        public Models.Municipios GetByIbgeCode(string codigoIbge);
        public Models.Municipios GetMunicipioById(long id);

    }
}
