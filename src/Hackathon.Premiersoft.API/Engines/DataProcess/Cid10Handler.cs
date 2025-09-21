using Hackathon.Premiersoft.API.Engines.DataProcess.Helpers;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using System;

namespace Hackathon.Premiersoft.API.Engines.DataProcess
{
    public class Cid10Handler : ICid10Handler
    {
        private IRepository<Cid10, Guid> Cid10Repository { get; set; }
        private IRepository<LineError, Guid> ErrorLineRepo { get; set; }

        public Cid10Handler(
            IRepository<Cid10, Guid> cid10Repository,
            IRepository<LineError, Guid> errorLineRepo)
        {
            Cid10Repository = cid10Repository;
            ErrorLineRepo = errorLineRepo;
        }

        public void ProcessarCid10(IEntityDto dto)
        {
            try
            {
                var cid10 = new Cid10();

                var tag = dto.Campo;
                var value = dto.Valor;

                if (DataHelper.CheckCampo(tag, nameof(cid10.Codigo)))
                    cid10.Codigo = value;

                if (DataHelper.CheckCampo(tag, nameof(cid10.Descricao)))
                    cid10.Descricao = value;

                Cid10Repository.Add(cid10);
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
