using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Enums;

namespace Hackathon.Premiersoft.API.Services.ImportFiles
{
    public class ImportFilesService : IImportFilesService
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;

        public ImportFilesService(IPremiersoftHackathonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(ImportFilesRequest request, CancellationToken cancellationToken)
        {
            var import = Import.Create(request.DataType, request.FileFormat, request.FileName, request.S3PreSignedUrl);

            _dbContext.Imports.Add(import);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
