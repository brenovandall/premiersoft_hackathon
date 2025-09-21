using Hackathon.Premiersoft.API.Engines.DataProcess.Helpers;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Repository;
using System;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class EstadosHandler : IEstadosHandler
    {
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private IRepository<Estados, Guid> EstadosRepository { get; set; }

        public EstadosHandler(IRepository<LineError, Guid> errorLineRepo, IRepository<Estados, Guid> estadosRepository)
        {
            ErrorLineRepo = errorLineRepo;
            EstadosRepository = estadosRepository;
        }

        public void ProcessarEstados(IEntityDto dto)
        {
            try
            {
                var estado = new Estados();

                var tag = dto.Campo;
                var value = dto.Valor;

                if (DataHelper.CheckCampo(tag, nameof(estado.Codigo_uf)))
                    estado.Codigo_uf = int.TryParse(value, out int codigoUf) ? codigoUf : throw new Exception($"Valor inválido para Codigo_uf: {value}");

                if (DataHelper.CheckCampo(tag, nameof(estado.Uf)))
                    estado.Uf = value;

                if (DataHelper.CheckCampo(tag, nameof(estado.Nome)))
                    estado.Nome = value;

                if (DataHelper.CheckCampo(tag, nameof(estado.Latitude)))
                    estado.Latitude = decimal.TryParse(value, out decimal lat) ? lat : throw new Exception($"Valor inválido para Latitude: {value}");

                if (DataHelper.CheckCampo(tag, nameof(estado.Longitude)))
                    estado.Longitude = decimal.TryParse(value, out decimal lon) ? lon : throw new Exception($"Valor inválido para Longitude: {value}");

                if (DataHelper.CheckCampo(tag, nameof(estado.Regiao)))
                    estado.Regiao = value;

                EstadosRepository.Add(estado);
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
