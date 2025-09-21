using Hackathon.Premiersoft.API.Dto;

namespace Hackathon.Premiersoft.API.Services.ImportFiles
{
    public interface IImportFilesService
    {
        Task Create(ImportFilesRequest request, CancellationToken cancellationToken);
    }
}
