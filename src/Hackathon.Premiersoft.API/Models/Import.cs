using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hackathon.Premiersoft.API.Models
{
    public class Import : Entity<Guid>
    {
        [Required]
        public ImportDataTypes DataType { get; private set; }

        [Required]
        public ImportFileFormat FileFormat { get; private set; }

        [Required]
        [MaxLength(100)]
        public string FileName { get; private set; } = default!;

        [Required]
        public string S3PreSignedUrl { get; private set; } = default!;

        [Required]
        public ImportStatus Status { get; private set; }

        public int? TotalRegisters { get; private set; }

        public int? TotalImportedRegisters { get; private set; }

        public int? TotalDuplicatedRegisters { get; private set; }

        public int? TotalFailedRegisters { get; private set; }

        [Required]
        public DateTime ImportedOn { get; private set; }

        public DateTime? FinishedOn { get; private set; }

        public ICollection<LineError> LineErrors { get; private set; } = [];

        protected Import() { }

        public Import(
            ImportDataTypes dataType,
            ImportFileFormat fileFormat,
            string fileName,
            string s3PreSignedUrl,
            ImportStatus status,
            int? totalRegisters,
            int? totalImportedRegisters,
            int? totalDuplicatedRegisters,
            int? totalFailedRegisters,
            DateTime importedOn,
            DateTime? finishedOn = null)
        {
            Id = Id = Guid.NewGuid();
            DataType = dataType;
            FileFormat = fileFormat;
            FileName = fileName;
            S3PreSignedUrl = s3PreSignedUrl;
            Status = status;
            TotalRegisters = totalRegisters;
            TotalImportedRegisters = totalImportedRegisters;
            TotalDuplicatedRegisters = totalDuplicatedRegisters;
            TotalFailedRegisters = totalFailedRegisters;
            ImportedOn = importedOn;
            FinishedOn = finishedOn;
        }

        public static Import Create(
            ImportDataTypes dataType,
            ImportFileFormat fileFormat,
            string fileName,
            string s3PreSignedUrl,
            int? totalRegisters = 0,
            int? totalImportedRegisters = 0,
            int? totalDuplicatedRegisters = 0,
            int? totalFailedRegisters = 0,
            DateTime? importedOn = null,
            DateTime? finishedOn = null)
        {
            var import = new Import(
                dataType: dataType,
                fileFormat: fileFormat,
                fileName: fileName,
                s3PreSignedUrl: s3PreSignedUrl,
                status: ImportStatus.Pending,
                totalRegisters: totalRegisters,
                totalImportedRegisters: totalImportedRegisters,
                totalDuplicatedRegisters: totalDuplicatedRegisters,
                totalFailedRegisters: totalFailedRegisters,
                importedOn: importedOn ?? DateTime.UtcNow,
                finishedOn: finishedOn);

            import.Raise(new ImportFileEvent(import.S3PreSignedUrl, (int)import.FileFormat, import.Id));

            return import;
        }

        public void UpdateStatus(ImportStatus status)
        {
            Status = status;
        }
    }
}
