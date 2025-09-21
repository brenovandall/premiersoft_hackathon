using Hackathon.Premiersoft.API.Engines.DataProcess.Helpers;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Repository.Municipios;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class MuncipiosHandler : IMuncipiosHandler
    {
        private IRepository<Municipios, Guid> MunicipiosRepository { get; set; }
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        public MuncipiosHandler(IRepository<Municipios, Guid> municipioRepo, IRepository<LineError, Guid> errorLineRepo)
        {
            MunicipiosRepository = municipioRepo;
            ErrorLineRepo = errorLineRepo;
        }
        public void ProcessarMuncipios(IEntityDto dto)
        {
            try
            {
                var municipio = new Municipios();

                var campoArquivo = dto.Campo;
                var value = dto.Valor;

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Populacao)))
                    municipio.Populacao = int.TryParse(value, out int populacao) ? populacao : throw new Exception($"Não foi possivel converter o municipio, verifique o valores nos campos.");

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Siafi_id)))
                    municipio.Siafi_id = int.TryParse(value, out int siafi) ? siafi : throw new Exception($"Não foi possivel converter o municipio, verifique o valores nos campos."); ;

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Codigo_ibge)))
                    municipio.Codigo_ibge = value;

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Codigo_uf)))
                    municipio.Codigo_uf = value;

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Fuso_horario)))
                    municipio.Fuso_horario = value;

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Codigo_uf)))
                    municipio.Ddd = int.TryParse(value, out int ddd) ? ddd : throw new Exception($"Não foi possivel converter o ddd, verifique o valores nos campos."); ;

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Longitude)))
                    municipio.Longitude = decimal.TryParse(value, out decimal longiture) ? longiture : throw new Exception($"Não foi possivel converter a longitude para o devido tipo, verifique o valores nos campos.");

                if (DataHelper.CheckCampo(campoArquivo, nameof(municipio.Latitude)))
                    municipio.Latitude = decimal.TryParse(value, out decimal latidude) ? latidude : throw new Exception($"Não foi possivel converter a latitude para o devido tipo, verifique o valores nos campos.");

                MunicipiosRepository.Add(municipio);
            }
            catch (Exception ex)
            {

                var error = new LineError(
                    importId: dto.Import.Id,
                    import: dto.Import,
                    line: dto.NumeroLinha,
                    field: dto.Campo,
                    error: ex.Message,
                    value: dto.Valor
                );

                ErrorLineRepo.Add(error);
            }
        }
    }
}
