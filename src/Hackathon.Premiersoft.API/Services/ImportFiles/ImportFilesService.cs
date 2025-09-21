using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;

namespace Hackathon.Premiersoft.API.Services.ImportFiles
{
    public class ImportFilesService : IImportFilesService
    {
        private readonly IPremiersoftHackathonDbContext _dbContext;
        private readonly IRepository<Import, Guid> _repository;

        public ImportFilesService(IPremiersoftHackathonDbContext dbContext, IRepository<Import, Guid> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public async Task<Import> Create(ImportFilesRequest request, CancellationToken cancellationToken)
        {
            var import = Import.Create(request.DataType, request.FileFormat, request.FileName, request.S3PreSignedUrl);
            var lookups = request.FieldMappings;

            foreach (var lookup in lookups)
            {
                var newLookup = new Lookup(import.Id, import, lookup.From, lookup.To);
                _dbContext.Lookups.Add(newLookup);
            }

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
