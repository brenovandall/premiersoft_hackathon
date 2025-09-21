using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Services.ImportFiles
{
    public interface IImportFilesService
    {
        Task Create(ImportFilesRequest request, CancellationToken cancellationToken);
        IList<Import> GetAll();
    }
}
