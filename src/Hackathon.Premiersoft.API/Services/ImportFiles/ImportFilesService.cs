using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Services.ImportFiles
{
    public class ImportFilesService : IImportFilesService
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly IRepository<Import, long> _repository;

        public ImportFilesService(IPremiersoftHackathonDbContext dbContext, IRepository<Import, long> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public async Task<Import> Create(ImportFilesRequest request, CancellationToken cancellationToken)
        {
            var import = Import.Create(request.DataType, request.FileFormat, request.FileName, request.S3PreSignedUrl);

            _dbContext.Imports.Add(import);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return import;
        }

        public IList<Import> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
