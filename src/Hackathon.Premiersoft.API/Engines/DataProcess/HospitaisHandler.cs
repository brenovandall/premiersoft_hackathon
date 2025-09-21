using Hackathon.Premiersoft.API.Engines.DataProcess.Helpers;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;
using System;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class HospitaisHandler : IHospitaisHandler
    {
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }
        private IRepository<Hospitais, Guid> HospitaisRepository { get; set; }
        private IMunicipiosRepository MunicipiosRepository { get; set; }

        public HospitaisHandler(
            IRepository<LineError, Guid> errorLineRepo,
            IRepository<Hospitais, Guid> hospitaisRepository,
            IMunicipiosRepository municipiosRepository)
        {
            ErrorLineRepo = errorLineRepo;
            HospitaisRepository = hospitaisRepository;
            MunicipiosRepository = municipiosRepository;
        }

        public void ProcessarHospitais(IEntityDto dto)
        {
            try
            {
                var hospital = new Hospitais();

                var tag = dto.Campo;
                var value = dto.Valor;

                if (DataHelper.CheckCampo(tag, nameof(hospital.Codigo)))
                    hospital.Codigo = value;

                if (DataHelper.CheckCampo(tag, nameof(hospital.Nome)))
                    hospital.Nome = value;

                if (DataHelper.CheckCampo(tag, nameof(hospital.Bairro)))
                    hospital.Bairro = value;

                if (DataHelper.CheckCampo(tag, nameof(hospital.Cidade)))
                {
                    var municipio = MunicipiosRepository.GetByIbgeCode(value);

                    if (municipio == null)
                        throw new Exception($"Município não encontrado para código IBGE {value}");

                    hospital.Cidade = municipio;
                }

                if (DataHelper.CheckCampo(tag, nameof(hospital.Leitos_totais)))
                    hospital.Leitos_totais = long.TryParse(value, out long leitos) ? leitos : throw new Exception($"Valor inválido para Leitos_totais: {value}");

                HospitaisRepository.Add(hospital);
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
